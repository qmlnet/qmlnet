#include <QmlNet/types/NetReference.h>
#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetJsValue.h>
#include <QmlNet/qml/NetQObject.h>
#include <QmlNetUtilities.h>
#include <QDateTime>
#include <QDebug>
#include <QJSEngine>
#include <QmlNet/qml/QQmlApplicationEngine.h>

namespace
{
struct NetReferenceQmlContainer
{
    QSharedPointer<NetReference> netReference;
};

struct NetJsValueQmlContainer
{
    QSharedPointer<NetJSValue> jsValue;
};

struct NetQObjectQmlContainer
{
    QSharedPointer<NetQObject> netQObject;
};

struct NetVariantListQmlContainer
{
    QSharedPointer<NetVariantList> netVariantList;
};
}

Q_DECLARE_METATYPE(NetReferenceQmlContainer)
Q_DECLARE_METATYPE(NetJsValueQmlContainer)
Q_DECLARE_METATYPE(NetQObjectQmlContainer)
Q_DECLARE_METATYPE(NetVariantListQmlContainer)

namespace
{
const int NetReferenceQmlContainerTypeId = qMetaTypeId<NetReferenceQmlContainer>();
const int NetJsValueQmlContainerTypeId = qMetaTypeId<NetJsValueQmlContainer>();
const int NetQObjectQmlContainerTypeId = qMetaTypeId<NetQObjectQmlContainer>();
const int NetVariantListQmlContainerTypeId = qMetaTypeId<NetVariantListQmlContainer>();
}

NetVariant::NetVariant() = default;

NetVariant::~NetVariant()
{
    clearNetReference();
}

NetVariantTypeEnum NetVariant::getVariantType() const
{
    const int type = variant.userType();
    switch(type) {
    case QMetaType::UnknownType:
        return NetVariantTypeEnum_Invalid;
    case QMetaType::Nullptr:
        return NetVariantTypeEnum_Null;
    case QMetaType::Bool:
        return NetVariantTypeEnum_Bool;
    case QMetaType::QChar:
        return NetVariantTypeEnum_Char;
    case qMetaTypeId<qint32>():
        return NetVariantTypeEnum_Int;
    case qMetaTypeId<quint32>():
        return NetVariantTypeEnum_UInt;
    case qMetaTypeId<qint64>():
        return NetVariantTypeEnum_Long;
    case qMetaTypeId<quint64>():
        return NetVariantTypeEnum_ULong;
    case QMetaType::Float:
        return NetVariantTypeEnum_Float;
    case QMetaType::Double:
        return NetVariantTypeEnum_Double;
    case QMetaType::QString:
        return NetVariantTypeEnum_String;
    case QMetaType::QDateTime:
        return NetVariantTypeEnum_DateTime;
    default:
        if(type == NetReferenceQmlContainerTypeId) {
            return NetVariantTypeEnum_Object;
        }
        else if(type == NetJsValueQmlContainerTypeId) {
            return NetVariantTypeEnum_JSValue;
        }
        else if(type == NetQObjectQmlContainerTypeId) {
            return NetVariantTypeEnum_QObject;
        }
        else if(type == NetVariantListQmlContainerTypeId) {
            return NetVariantTypeEnum_NetVariantList;
        }
        else {
            qWarning() << "Unknown type for NetVariant: " << variant.typeName();
            return NetVariantTypeEnum_Invalid;
        }
    }
}

void NetVariant::setNull()
{
    clearNetReference();
    variant.setValue(nullptr);
}

void NetVariant::setNetReference(QSharedPointer<NetReference> netReference)
{
    clearNetReference();
    variant.setValue(NetReferenceQmlContainer{ std::move(netReference) });
}

QSharedPointer<NetReference> NetVariant::getNetReference() const
{
    return getValue<NetReferenceQmlContainer>().netReference;
}

void NetVariant::setBool(bool value)
{
    setValue(value);
}

bool NetVariant::getBool() const
{
    return getValue<bool>();
}

void NetVariant::setChar(QChar value)
{
    setValue(value);
}

QChar NetVariant::getChar() const
{
    return getValue<QChar>();
}

void NetVariant::setInt(qint32 value)
{
    setValue(value);
}

qint32 NetVariant::getInt() const
{
    return getValue<qint32>();
}

void NetVariant::setUInt(quint32 value)
{
    setValue(value);
}

quint32 NetVariant::getUInt() const
{
    return getValue<quint32>();
}

void NetVariant::setLong(qint64 value)
{
    setValue(value);
}

qint64 NetVariant::getLong() const
{
    return getValue<qint64>();
}

void NetVariant::setULong(quint64 value)
{
    setValue(value);
}

quint64 NetVariant::getULong() const
{
    return getValue<quint64>();
}

void NetVariant::setFloat(float value)
{
    setValue(value);
}

float NetVariant::getFloat() const
{
    return getValue<float>();
}

void NetVariant::setDouble(double value)
{
    setValue(value);
}

double NetVariant::getDouble() const
{
    return getValue<double>();
}

void NetVariant::setString(const QString* value)
{
    setValuePtr(value);
}

void NetVariant::setString(const QString& value)
{
    setValue(value);
}

QString NetVariant::getString() const
{
    if(variant.userType() != QMetaType::QString) {
        qDebug() << "Variant is not a string";
        return QString();
    }

    return variant.toString();
}

void NetVariant::setDateTime(const QDateTime& value)
{
    setValue(value);
}

QDateTime NetVariant::getDateTime() const
{
    return getValue<QDateTime>();
}

void NetVariant::setJsValue(QSharedPointer<NetJSValue> jsValue)
{
    setValue(NetJsValueQmlContainer{ std::move(jsValue) });
}

QSharedPointer<NetJSValue> NetVariant::getJsValue() const
{
    return getValue<NetJsValueQmlContainer>().jsValue;
}

void NetVariant::setQObject(QSharedPointer<NetQObject> netQObject)
{
    setValue(NetQObjectQmlContainer{ std::move(netQObject) });
}

QSharedPointer<NetQObject> NetVariant::getQObject() const
{
    return getValue<NetQObjectQmlContainer>().netQObject;
}

void NetVariant::setNetVariantList(QSharedPointer<NetVariantList> netVariantList)
{
    setValue(NetVariantListQmlContainer{ std::move(netVariantList) });
}

QSharedPointer<NetVariantList> NetVariant::getNetVariantList() const
{
    return getValue<NetVariantListQmlContainer>().netVariantList;
}

void NetVariant::clear()
{
    clearNetReference();
    variant.clear();
}

QVariantList NetVariant::toQVariantList() const
{
    NetVariantTypeEnum variantType = getVariantType();

    if(variantType == NetVariantTypeEnum_NetVariantList) {
        QVariantList list;

        QSharedPointer<NetVariantList> netVariantList = getValue<NetVariantListQmlContainer>().netVariantList;
        for(int x = 0; x < netVariantList->count(); x++) {
            QSharedPointer<NetVariant> variant = netVariantList->get(x);
            list.append(variant->toQVariant());
        }

        return list;
    }

    if(variantType == NetVariantTypeEnum_Object) {
        // This may be a .NET list type.
        // If it is, try to enumerate it.
        QSharedPointer<NetReference> netReference = getNetReference();

        QSharedPointer<NetTypeArrayFacade> facade = netReference->getTypeInfo()->getArrayFacade();
        if(facade == nullptr) {
            qWarning() << "The given .NET type" << netReference->getTypeInfo()->getClassName() << "can't be converted to a QVariantList";
            return QVariantList();
        }

        QVariantList list;
        uint count = facade->getLength(netReference);
        for(uint x = 0; x < count; x++) {
            QSharedPointer<NetVariant> item = facade->getIndexed(netReference, x);
            list.append(item->toQVariant());
        }
        return list;
    }

    qWarning() << "Can't convert value" << variant << "from" << variant.typeName() << "to QVariantList";

    return QVariantList();
}

QSharedPointer<NetVariant> NetVariant::fromQJSValue(const QJSValue& qJsValue)
{
    QSharedPointer<NetVariant> result;
    if(qJsValue.isNull() || qJsValue.isUndefined()) {
        // Nothing!
    }
    else if(qJsValue.isQObject()) {
        result = QSharedPointer<NetVariant>(new NetVariant());
        QObject* qObject = qJsValue.toQObject();
        NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);
        if(!netValue) {
            result->setQObject(QSharedPointer<NetQObject>(new NetQObject(qObject)));
        } else {
            result->setNetReference(netValue->getNetReference());
        }
    }
    else if(qJsValue.isObject()) {
        result = QSharedPointer<NetVariant>(new NetVariant());
        result->setJsValue(QSharedPointer<NetJSValue>(new NetJSValue(qJsValue)));
    } else {
        result = QSharedPointer<NetVariant>(new NetVariant());
        QVariant variant = qJsValue.toVariant();
        result->variant = variant;
    }
    return result;
}

QJSValue NetVariant::toQJSValue() const
{
    switch(getVariantType()) {
    case NetVariantTypeEnum_Object: {
        NetValue* netValue = NetValue::forInstance(getNetReference());
        return sharedQmlEngine()->newQObject(netValue);
    }
    case NetVariantTypeEnum_JSValue: {
        return getJsValue()->getJsValue();
    }
    default: {
        return sharedQmlEngine()->toScriptValue<QVariant>(toQVariant());
    }
    }
}

void NetVariant::fromQVariant(const QVariant* variant, const QSharedPointer<NetVariant>& destination)
{
    const int type = variant->userType();
    switch(type) {
    case QMetaType::UnknownType:
        destination->clear();
        break;
    case QMetaType::Bool:
    case QMetaType::QChar:
    case qMetaTypeId<qint32>():
    case qMetaTypeId<quint32>():
    case qMetaTypeId<qint64>():
    case qMetaTypeId<quint64>():
    case QMetaType::Float:
    case QMetaType::Double:
    case QMetaType::QString:
    case QMetaType::QDateTime:
        destination->setValueVariant(*variant);
        break;
    case QMetaType::ULong:
        destination->setULong(variant->value<quint64>());
        break;
    case QMetaType::Long:
        destination->setLong(variant->value<qint64>());
        break;
    case QMetaType::QObjectStar: {
        QObject* value = variant->value<QObject*>();
        if(value == nullptr) {
            destination->clear();
            return;
        }
        NetValueInterface* netValue = qobject_cast<NetValueInterface*>(value);
        if(netValue) {
            destination->setNetReference(netValue->getNetReference());
        } else {
            destination->setQObject(QSharedPointer<NetQObject>(new NetQObject(value)));
        }
        break;
    }
    case QMetaType::QVariantList: {
        QSharedPointer<NetVariantList> netVariantList = QSharedPointer<NetVariantList>(new NetVariantList());
        QVariantList list = variant->value<QVariantList>();
        QVariantList::iterator i;
        for (i = list.begin(); i != list.end(); ++i) {
            QVariant item = *i;
            netVariantList->add(NetVariant::fromQVariant(&item));
        }
        destination->setNetVariantList(netVariantList);
        break;
    }
    default:
        if(type == qMetaTypeId<QJSValue>()) {
            // TODO: Either serialize this type to a string, to be deserialized in .NET, or
            // pass raw value to .NET to be dynamically invoked (using dynamic).
            // See qtdeclarative\src\plugins\qmltooling\qmldbg_debugger\qqmlenginedebugservice.cpp:184
            // for serialization methods.
            QSharedPointer<NetJSValue> netJsValue(new NetJSValue(variant->value<QJSValue>()));
            destination->setJsValue(netJsValue);
            break;
        }

        QMetaType::TypeFlags flags = QMetaType::typeFlags(type);
        if(flags & QMetaType::PointerToQObject) {
            QObject* value = variant->value<QObject*>();
            if(value == nullptr) {
                destination->clear();
                break;
            }
            destination->setQObject(QSharedPointer<NetQObject>(new NetQObject(value)));
            break;
        }

        qDebug() << "Unsupported variant type: " << variant->type() << variant->typeName();
        break;
    }
}

QSharedPointer<NetVariant> NetVariant::fromQVariant(const QVariant* variant)
{
    QSharedPointer<NetVariant> result(new NetVariant());
    fromQVariant(variant, result);
    return result;
}

QVariant NetVariant::toQVariant() const
{
    switch(getVariantType()) {
    case NetVariantTypeEnum_JSValue:
        return getJsValue()->getJsValue().toVariant();
    case NetVariantTypeEnum_Object:
        return QVariant::fromValue<QObject*>(NetValue::forInstance(getNetReference()));
    case NetVariantTypeEnum_QObject:
        return QVariant::fromValue<QObject*>(this->getQObject()->getQObject());
    case NetVariantTypeEnum_NetVariantList:
        return QVariant::fromValue(toQVariantList());
    default:
        return variant;
    }
}

QString NetVariant::getDisplayValue() const
{
    switch(getVariantType()) {
    case NetVariantTypeEnum_JSValue:
        return getJsValue()->getJsValue().toString();
    case NetVariantTypeEnum_Object:
        return getNetReference()->displayName();
    case NetVariantTypeEnum_QObject:
        return getQObject()->getQObject()->objectName();
    default:
        return variant.toString();
    }
}

void NetVariant::clearNetReference()
{
    if(variant.canConvert<NetReferenceQmlContainer>()) {
        variant.value<NetReferenceQmlContainer>().netReference.clear();
        variant.clear();
    }
    else if(variant.canConvert<NetJsValueQmlContainer>()) {
        variant.value<NetJsValueQmlContainer>().jsValue.clear();
        variant.clear();
    }
    else if(variant.canConvert<NetQObjectQmlContainer>()) {
        variant.value<NetQObjectQmlContainer>().netQObject.clear();
        variant.clear();
    }
}

template<typename T>
void NetVariant::setValue(const T& value)
{
    clearNetReference();
    variant.setValue(value);
}

void NetVariant::setValueVariant(const QVariant& value)
{
    Q_ASSERT(value.userType() != QMetaType::QObjectStar);
    Q_ASSERT(value.userType() != qMetaTypeId<QJSValue>());
    Q_ASSERT(value.userType() < QMetaType::User);
    clearNetReference();
    variant = value;
}

template<typename T>
void NetVariant::setValuePtr(const T* value)
{
    if(value) {
        setValue(*value);
    } else {
        clear();
    }
}

template<typename T>
T NetVariant::getValue() const
{
    if(!variant.canConvert(qMetaTypeId<T>())) {
        qDebug() << "Can't convert value" << variant << "from" << variant.typeName() << "to" << QMetaType::typeName(qMetaTypeId<T>());
    }
    return variant.value<T>();
}

extern "C" {

struct Q_DECL_EXPORT DateTimeContainer {
    uchar isNull;
    int month;
    int day;
    int year;
    int hour;
    int minute;
    int second;
    int msec;
    int offsetSeconds;
};

Q_DECL_EXPORT NetVariantContainer* net_variant_create() {
    NetVariantContainer* result = new NetVariantContainer();
    result->variant = QSharedPointer<NetVariant>(new NetVariant());
    return result;
}

Q_DECL_EXPORT void net_variant_destroy(NetVariantContainer* container) {
    delete container;
}

Q_DECL_EXPORT NetVariantTypeEnum net_variant_getVariantType(NetVariantContainer* container) {
    return container->variant->getVariantType();
}

Q_DECL_EXPORT void net_variant_setNull(NetVariantContainer* container) {
    container->variant->setNull();
}

Q_DECL_EXPORT void net_variant_setNetReference(NetVariantContainer* container, NetReferenceContainer* instanceContainer) {
    if(instanceContainer == nullptr) {
        container->variant->setNetReference(nullptr);
    } else {
        container->variant->setNetReference(instanceContainer->instance);
    }
}

Q_DECL_EXPORT NetReferenceContainer* net_variant_getNetReference(NetVariantContainer* container) {
    QSharedPointer<NetReference> instance = container->variant->getNetReference();
    if(instance == nullptr) {
        return nullptr;
    }
    NetReferenceContainer* result = new NetReferenceContainer();
    result->instance = instance;
    return result;
}

Q_DECL_EXPORT void net_variant_setBool(NetVariantContainer* container, uchar value) {
    container->variant->setBool(value == 1 ? true : false);
}

Q_DECL_EXPORT uchar net_variant_getBool(NetVariantContainer* container) {
    if(container->variant->getBool()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT void net_variant_setChar(NetVariantContainer* container, quint16 value) {
    container->variant->setChar(value);
}

Q_DECL_EXPORT quint16 net_variant_getChar(NetVariantContainer* container) {
    return quint16(container->variant->getChar().unicode());
}

Q_DECL_EXPORT void net_variant_setInt(NetVariantContainer* container, qint32 value) {
    container->variant->setInt(value);
}

Q_DECL_EXPORT qint32 net_variant_getInt(NetVariantContainer* container) {
    return container->variant->getInt();
}

Q_DECL_EXPORT void net_variant_setUInt(NetVariantContainer* container, quint32 value) {
    container->variant->setUInt(value);
}

Q_DECL_EXPORT quint32 net_variant_getUInt(NetVariantContainer* container) {
    return container->variant->getUInt();
}

Q_DECL_EXPORT void net_variant_setLong(NetVariantContainer* container, qint64 value) {
    container->variant->setLong(value);
}

Q_DECL_EXPORT qint64 net_variant_getLong(NetVariantContainer* container) {
    return container->variant->getLong();
}

Q_DECL_EXPORT void net_variant_setULong(NetVariantContainer* container, quint64 value) {
    container->variant->setULong(value);
}

Q_DECL_EXPORT quint64 net_variant_getULong(NetVariantContainer* container) {
    return container->variant->getULong();
}

Q_DECL_EXPORT void net_variant_setFloat(NetVariantContainer* container, float value) {
    container->variant->setFloat(value);
}

Q_DECL_EXPORT float net_variant_getFloat(NetVariantContainer* container) {
    return container->variant->getFloat();
}

Q_DECL_EXPORT void net_variant_setDouble(NetVariantContainer* container, double value) {
    container->variant->setDouble(value);
}

Q_DECL_EXPORT double net_variant_getDouble(NetVariantContainer* container) {
    return container->variant->getDouble();
}

Q_DECL_EXPORT void net_variant_setString(NetVariantContainer* container, LPWSTR value) {
    if(value == nullptr) {
        container->variant->setString(nullptr);
    } else {
        container->variant->setString(QString::fromUtf16(static_cast<const char16_t*>(value)));
    }
}

Q_DECL_EXPORT QmlNetStringContainer* net_variant_getString(NetVariantContainer* container) {
    const QString& string = container->variant->getString();
    if(string.isNull()) {
        return nullptr;
    }
    return createString(string);
}

Q_DECL_EXPORT void net_variant_setDateTime(NetVariantContainer* container, const DateTimeContainer* value) {
    if(value == nullptr || value->isNull) {
        container->variant->setDateTime(QDateTime());
    } else {
        container->variant->setDateTime(QDateTime(QDate(value->year, value->month, value->day),
                                                  QTime(value->hour, value->minute, value->second, value->msec),
                                                  Qt::OffsetFromUTC, value->offsetSeconds));
    }
}
Q_DECL_EXPORT void net_variant_getDateTime(NetVariantContainer* container, DateTimeContainer* value) {
    const QDateTime& dt = container->variant->getDateTime();
    if(dt.isNull()) {
        value->isNull = 1;
        return;
    }
    if(!dt.isValid()) {
        qWarning() << "QDateTime is invalid";
        value->isNull = 1;
        return;
    }
    value->isNull = 0;
    const QDate& date = dt.date();
    const QTime& time = dt.time();
    value->year = date.year();
    value->month = date.month();
    value->day = date.day();
    value->hour = time.hour();
    value->minute = time.minute();
    value->second = time.second();
    value->msec = time.msec();
    value->offsetSeconds = dt.offsetFromUtc();
}

Q_DECL_EXPORT void net_variant_setJsValue(NetVariantContainer* container, NetJSValueContainer* jsValueContainer) {
    if(jsValueContainer == nullptr) {
        container->variant->setJsValue(nullptr);
    } else {
        container->variant->setJsValue(jsValueContainer->jsValue);
    }
}

Q_DECL_EXPORT NetJSValueContainer* net_variant_getJsValue(NetVariantContainer* container) {
    const QSharedPointer<NetJSValue>& instance = container->variant->getJsValue();
    if(instance == nullptr) {
        return nullptr;
    }
    NetJSValueContainer* result = new NetJSValueContainer();
    result->jsValue = instance;
    return result;
}

Q_DECL_EXPORT void net_variant_setQObject(NetVariantContainer* container, NetQObjectContainer* qObjectContainer) {
    if(qObjectContainer == nullptr) {
        container->variant->setQObject(nullptr);
    } else {
        container->variant->setQObject(qObjectContainer->qObject);
    }
}

Q_DECL_EXPORT NetQObjectContainer* net_variant_getQObject(NetVariantContainer* container) {
    const QSharedPointer<NetQObject>& instance = container->variant->getQObject();
    if(instance == nullptr) {
        return nullptr;
    }
    NetQObjectContainer* result = new NetQObjectContainer();
    result->qObject = instance;
    return result;
}

Q_DECL_EXPORT void net_variant_setNetVariantList(NetVariantContainer* container, NetVariantListContainer* netVariantListContainer) {
    if(netVariantListContainer == nullptr) {
        container->variant->setNetVariantList(nullptr);
    } else {
        container->variant->setNetVariantList(netVariantListContainer->list);
    }
}

Q_DECL_EXPORT NetVariantListContainer* net_variant_getNetVariantList(NetVariantContainer* container) {
    const QSharedPointer<NetVariantList>& netVariantList = container->variant->getNetVariantList();
    if(netVariantList == nullptr) {
        return nullptr;
    }
    return new NetVariantListContainer { netVariantList };
}

Q_DECL_EXPORT void net_variant_clear(NetVariantContainer* container) {
    container->variant->clear();
}

}

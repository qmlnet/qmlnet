#include <QmlNet/qml/NetVariant.h>
#include <QDateTime>
#include <QDebug>

struct NetReferenceQmlContainer
{
    QSharedPointer<NetReference> NetReference;
};

Q_DECLARE_METATYPE(NetReferenceQmlContainer)

NetVariant::NetVariant()
{

}

NetVariant::~NetVariant()
{
    clearNetReference();
}

NetVariantTypeEnum NetVariant::getVariantType()
{
    switch(variant.type()) {
    case QVariant::Invalid:
        return NetVariantTypeEnum_Invalid;
    case QVariant::Bool:
        return NetVariantTypeEnum_Bool;
    case QVariant::Char:
        return NetVariantTypeEnum_Char;
    case QVariant::Int:
        return NetVariantTypeEnum_Int;
    case QVariant::UInt:
        return NetVariantTypeEnum_UInt;
    case QVariant::Double:
        return NetVariantTypeEnum_Double;
    case QVariant::String:
        return NetVariantTypeEnum_String;
    case QVariant::DateTime:
        return NetVariantTypeEnum_DateTime;
    case QVariant::UserType:
        if(strcmp(variant.typeName(), "NetReferenceQmlContainer") == 0)
            return NetVariantTypeEnum_Object;
        qWarning() << "Unknown user type for NetVariant: " << variant.typeName();
        return NetVariantTypeEnum_Invalid;
    default:
        qWarning() << "Unsupported qt variant type: " << variant.type();
        return NetVariantTypeEnum_Invalid;
    }
}

void NetVariant::setNetReference(QSharedPointer<NetReference> netReference)
{
    clearNetReference();
    variant.setValue(NetReferenceQmlContainer{ netReference });
}

QSharedPointer<NetReference> NetVariant::getNetReference()
{
    return variant.value<NetReferenceQmlContainer>().NetReference;
}

void NetVariant::setBool(bool value)
{
    clearNetReference();
    variant.setValue(value);
}

bool NetVariant::getBool()
{
    if(variant.canConvert(QMetaType::Bool))
        return variant.value<int>();

    qDebug() << "Can't convert value to bool";

    return false;
}

void NetVariant::setChar(QChar value)
{
    clearNetReference();
    variant.setValue(value);
}

QChar NetVariant::getChar()
{
    return variant.toChar();
}

void NetVariant::setInt(int value)
{
    clearNetReference();
    variant.setValue(value);
}

int NetVariant::getInt()
{
    if(variant.canConvert(QMetaType::Int))
        return variant.value<int>();

    qDebug() << "Can't convert value to int";

    return 0;
}

void NetVariant::setUInt(unsigned int value)
{
    clearNetReference();
    variant.setValue(value);
}

unsigned int NetVariant::getUInt()
{
    bool ok = false;
    unsigned int result = variant.toUInt(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to unsigned int";
    }

    return result;
}

void NetVariant::setDouble(double value)
{
    clearNetReference();
    variant.setValue(value);
}

double NetVariant::getDouble()
{
    bool ok = false;
    double result = variant.toDouble(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to double";
    }

    return result;
}

void NetVariant::setString(QString* value)
{
    clearNetReference();
    if(value) {
        variant.setValue(*value);
    } else {
        variant.clear();
    }
}

QString NetVariant::getString()
{
    if(variant.type() != QVariant::String) {
        qDebug() << "Variant is not a string";
        return QString();
    }

    return variant.toString();
}

void NetVariant::setDateTime(QDateTime& value)
{
    clearNetReference();
    if(value.isNull()) {
        variant.clear();
    } else {
        variant.setValue(value);
    }
}

QDateTime NetVariant::getDateTime()
{
    return variant.toDateTime();
}

void NetVariant::clear()
{
    clearNetReference();
    variant.clear();
}

QVariant NetVariant::asQVariant()
{
    return variant;
}

void NetVariant::clearNetReference()
{
    if(variant.canConvert<NetReferenceQmlContainer>())
    {
        variant.value<NetReferenceQmlContainer>().NetReference.clear();
        variant.clear();
    }
}

extern "C" {

struct Q_DECL_EXPORT DateTimeContainer {
    bool isNull;
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

Q_DECL_EXPORT void net_variant_setNetReference(NetVariantContainer* container, NetReferenceContainer* instanceContainer) {
    if(instanceContainer == NULL) {
        container->variant->setNetReference(NULL);
    } else {
        container->variant->setNetReference(instanceContainer->instance);
    }
}

Q_DECL_EXPORT NetReferenceContainer* net_variant_getNetReference(NetVariantContainer* container) {
    QSharedPointer<NetReference> instance = container->variant->getNetReference();
    if(instance == NULL) {
        return NULL;
    }
    NetReferenceContainer* result = new NetReferenceContainer();
    result->instance = instance;
    return result;
}

Q_DECL_EXPORT void net_variant_setBool(NetVariantContainer* container, bool value) {
    container->variant->setBool(value);
}

Q_DECL_EXPORT bool net_variant_getBool(NetVariantContainer* container) {
    return container->variant->getBool();
}

Q_DECL_EXPORT void net_variant_setChar(NetVariantContainer* container, ushort value) {
    container->variant->setChar(value);
}

Q_DECL_EXPORT ushort net_variant_getChar(NetVariantContainer* container) {
    return (ushort)container->variant->getChar().unicode();
}

Q_DECL_EXPORT void net_variant_setInt(NetVariantContainer* container, int value) {
    container->variant->setInt(value);
}

Q_DECL_EXPORT int net_variant_getInt(NetVariantContainer* container) {
    return container->variant->getInt();
}

Q_DECL_EXPORT void net_variant_setUInt(NetVariantContainer* container, unsigned int value) {
    container->variant->setUInt(value);
}

Q_DECL_EXPORT unsigned int net_variant_getUInt(NetVariantContainer* container) {
    return container->variant->getUInt();
}

Q_DECL_EXPORT void net_variant_setDouble(NetVariantContainer* container, double value) {
    container->variant->setDouble(value);
}

Q_DECL_EXPORT double net_variant_getDouble(NetVariantContainer* container) {
    return container->variant->getDouble();
}

Q_DECL_EXPORT void net_variant_setString(NetVariantContainer* container, LPWSTR value) {
    if(value == NULL) {
        container->variant->setString(NULL);
    } else {
        QString temp = QString::fromUtf16((const char16_t*)value);
        container->variant->setString(&temp);
    }
}

Q_DECL_EXPORT LPWSTR net_variant_getString(NetVariantContainer* container) {
    QString string = container->variant->getString();
    if(string.isNull()) {
        return NULL;
    }
    return (LPWSTR)string.utf16();
}

Q_DECL_EXPORT void net_variant_setDateTime(NetVariantContainer* container, DateTimeContainer* value) {
    if(value == NULL || value->isNull) {
        QDateTime dt;
        container->variant->setDateTime(dt);
    } else {
        QDateTime dt(QDate(value->year, value->month, value->day),
                     QTime(value->hour, value->minute, value->second, value->msec),
                     Qt::OffsetFromUTC, value->offsetSeconds);
        container->variant->setDateTime(dt);
    }
}
Q_DECL_EXPORT void net_variant_getDateTime(NetVariantContainer* container, DateTimeContainer* value) {
    QDateTime dt = container->variant->getDateTime();
    if(dt.isNull()) {
        value->isNull = true;
        return;
    }
    if(!dt.isValid()) {
        qWarning("QDateTime is invalid");
        value->isNull = true;
        return;
    }
    value->year = dt.date().year();
    value->month = dt.date().month();
    value->day = dt.date().day();
    value->hour = dt.time().hour();
    value->minute = dt.time().minute();
    value->second = dt.time().second();
    value->msec = dt.time().msec();
    value->offsetSeconds = dt.offsetFromUtc();
}

Q_DECL_EXPORT void net_variant_clear(NetVariantContainer* container) {
    container->variant->clear();
}

}

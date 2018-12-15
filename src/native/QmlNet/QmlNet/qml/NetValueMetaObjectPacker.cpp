#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetValue.h>
#include <QDebug>

const char* NetValueTypePacker::getQmlType()
{
    return "QVariant";
}

void NetValueTypePacker::pack(QSharedPointer<NetVariant> source, void* destination)
{
    QVariant* destinationVariant = static_cast<QVariant*>(destination);
    switch(source->getVariantType()) {
    case NetVariantTypeEnum_Invalid:
        destinationVariant->setValue(QVariant::fromValue(nullptr));
        break;
    case NetVariantTypeEnum_Bool:
        destinationVariant->setValue(source->getBool());
        break;
    case NetVariantTypeEnum_Char:
        destinationVariant->setValue(source->getChar());
        break;
    case NetVariantTypeEnum_Int:
        destinationVariant->setValue(source->getInt());
        break;
    case NetVariantTypeEnum_UInt:
        destinationVariant->setValue(source->getUInt());
        break;
    case NetVariantTypeEnum_Long:
        destinationVariant->setValue(source->getLong());
        break;
    case NetVariantTypeEnum_ULong:
        destinationVariant->setValue(source->getULong());
        break;
    case NetVariantTypeEnum_Float:
        destinationVariant->setValue(source->getFloat());
        break;
    case NetVariantTypeEnum_Double:
        destinationVariant->setValue(source->getDouble());
        break;
    case NetVariantTypeEnum_String:
        destinationVariant->setValue(source->getString());
        break;
    case NetVariantTypeEnum_DateTime:
        destinationVariant->setValue(source->getDateTime());
        break;
    case NetVariantTypeEnum_Object:
    {
        QSharedPointer<NetReference> newInstance = source->getNetReference();
        NetValue* netValue = NetValue::forInstance(newInstance);
        destinationVariant->setValue(netValue);
        break;
    }
    case NetVariantTypeEnum_JSValue:
        destinationVariant->setValue(source->getJsValue()->getJsValue());
        break;
    }
}

void NetValueTypePacker::unpack(QSharedPointer<NetVariant> destination, void* source, NetVariantTypeEnum prefType)
{
    QVariant* sourceVariant = static_cast<QVariant*>(source);

    if(sourceVariant->isNull()) {
        destination->clear();
        return;
    }

    switch(prefType) {
    case NetVariantTypeEnum_Invalid:
        destination->clear();
        return;
    case NetVariantTypeEnum_Bool:
        destination->setBool(sourceVariant->toBool());
        return;
    case NetVariantTypeEnum_Char: {
        const QString& v = sourceVariant->toString();
        if(v.length() == 1) {
            destination->setChar(v.at(0));
        } else {
            qDebug() << "Can't set string" << v << "to char.";
            destination->setChar(QChar::Null);
        }
        return;
    }
    case NetVariantTypeEnum_Int:
        destination->setInt(sourceVariant->value<qint32>());
        return;
    case NetVariantTypeEnum_UInt:
        destination->setUInt(sourceVariant->value<quint32>());
        return;
    case NetVariantTypeEnum_Long:
        destination->setLong(sourceVariant->value<qint64>());
        return;
    case NetVariantTypeEnum_ULong:
        destination->setULong(sourceVariant->value<quint64>());
        return;
    case NetVariantTypeEnum_Float:
        destination->setFloat(sourceVariant->toFloat());
        return;
    case NetVariantTypeEnum_Double:
        destination->setDouble(sourceVariant->toDouble());
        return;
    case NetVariantTypeEnum_String:
        destination->setString(sourceVariant->toString());
        return;
    case NetVariantTypeEnum_DateTime:
        destination->setDateTime(sourceVariant->toDateTime());
        return;
    case NetVariantTypeEnum_Object:
    {
        if(sourceVariant->userType() == QMetaType::QObjectStar) {
            QObject* value = sourceVariant->value<QObject*>();
            NetValueInterface* netValue = qobject_cast<NetValueInterface*>(value);
            if(netValue) {
                destination->setNetReference(netValue->getNetReference());
                return;
            }
        }
        break;
    }
    case NetVariantTypeEnum_JSValue:
    {
        if(sourceVariant->userType() == qMetaTypeId<QJSValue>()) {
            QSharedPointer<NetJSValue> netJsValue(new NetJSValue(sourceVariant->value<QJSValue>()));
            destination->setJsValue(netJsValue);
            return;
        } else {
            // TODO: Try to convert other types to JS Value.
        }
        break;
    }
    }

    NetVariant::fromQVariant(sourceVariant, destination);
}

class StringValueTypePacker : public NetValueTypePacker
{
public:
    const char* getQmlType()
    {
        return "QString";
    }
    void pack(QSharedPointer<NetVariant> source, void* destination)
    {
        Q_ASSERT(source->getVariantType() == NetVariantTypeEnum_String);
        QString* destinationString = static_cast<QString*>(destination);
        switch(source->getVariantType()){
        case NetVariantTypeEnum_Invalid:
            destinationString->clear();
            break;
        case NetVariantTypeEnum_String:
            *destinationString = source->getString();
            break;
        default:
            qWarning("Attempting to set a variant id %d to a QString", source->getVariantType());
            break;
        }
    }
    void unpack(QSharedPointer<NetVariant> destination, void* source, NetVariantTypeEnum prefType)
    {
        Q_ASSERT(prefType == NetVariantTypeEnum_String);
        QString* sourceString = static_cast<QString*>(source);
        destination->setString(sourceString);
    }
};

NetValueMetaObjectPacker::NetValueMetaObjectPacker()
{
    NetValueTypePacker* variantPacker = new NetValueTypePacker();
    StringValueTypePacker* stringPacker = new StringValueTypePacker();

    //This is might not be pretty, but it does allow the compiler generate a warning if a value is missing.
    for (int typeInt = NetVariantTypeEnum_Invalid; typeInt <= NetVariantTypeEnum_JSValue; ++typeInt)
    {
        NetVariantTypeEnum type = NetVariantTypeEnum(typeInt);
        switch(type) {
        case NetVariantTypeEnum_Invalid:
        case NetVariantTypeEnum_Bool:
        case NetVariantTypeEnum_Char:
        case NetVariantTypeEnum_Int:
        case NetVariantTypeEnum_UInt:
        case NetVariantTypeEnum_Long:
        case NetVariantTypeEnum_ULong:
        case NetVariantTypeEnum_Float:
        case NetVariantTypeEnum_Double:
        case NetVariantTypeEnum_DateTime:
        case NetVariantTypeEnum_Object:
        case NetVariantTypeEnum_JSValue:
            packers[type] = variantPacker;
            break;
        case NetVariantTypeEnum_String:
            packers[type] = stringPacker;
            break;
        }
    }
}

NetValueMetaObjectPacker* NetValueMetaObjectPacker::getInstance()
{
    static NetValueMetaObjectPacker packer;
    return &packer;
}

NetValueTypePacker* NetValueMetaObjectPacker::getPacker(NetVariantTypeEnum variantType)
{
    Q_ASSERT(packers.contains(variantType));
    return packers[variantType];
}

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
        destinationVariant->clear();
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
    default:
        qWarning() << "Unsupported variant type: " << source->getVariantType();
        break;
    }
}

void NetValueTypePacker::unpack(QSharedPointer<NetVariant> destination, void* source, NetVariantTypeEnum prefType)
{
    QVariant* sourceVariant = static_cast<QVariant*>(source);
    bool ok = false;

    if(sourceVariant->isNull()) {
        destination->clear();
        return;
    }

    switch(prefType) {
    case NetVariantTypeEnum_Bool:
        destination->setBool(sourceVariant->toBool());
        return;
    case NetVariantTypeEnum_Char: {
        QString v = sourceVariant->toString();
        if(v.isNull() || v.isEmpty()) {
            qDebug() << "Can't set empty string to char, setting to null.";
            destination->setChar(QChar::Null);
        } else {
            if(v.length() == 1) {
                destination->setChar(v.at(0));
            } else {
                qDebug() << "Can't set string to char that has more than one character.";
                destination->setChar(QChar::Null);
            }
        }
        return;
    }
    case NetVariantTypeEnum_Int:
    {
        int result = sourceVariant->toInt(&ok);
        if(ok) {
            destination->setInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_UInt:
    {
        unsigned int result = sourceVariant->toUInt(&ok);
        if(ok) {
            destination->setUInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_Double:
    {
        double result = sourceVariant->toDouble(&ok);
        if(ok) {
            destination->setDouble(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_String:
    {
        QString stringResult = sourceVariant->toString();
        destination->setString(&stringResult);
        return;
    }
    case NetVariantTypeEnum_DateTime:
    {
        QDateTime dateTimeResult = sourceVariant->toDateTime();
        if(!dateTimeResult.isValid()) {
            qDebug() << "Invalid date time";
            break;
        }
        if(dateTimeResult.isNull()) {
            destination->clear();
            break;
        }
        destination->setDateTime(dateTimeResult);
        return;
    }
    case NetVariantTypeEnum_Object:
    {
        if (sourceVariant->type() == static_cast<QVariant::Type>(QMetaType::QObjectStar)) {

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
            QSharedPointer<NetJSValue> netJsValue = QSharedPointer<NetJSValue>(new NetJSValue(sourceVariant->value<QJSValue>()));
            destination->setJsValue(netJsValue);
            return;
        } else {
            // TODO: Try to convert other types to JS Value.
        }
        break;
    }
    default:
        break;
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
        QString* destinationString = static_cast<QString*>(destination);
        switch(source->getVariantType()){
        case NetVariantTypeEnum_Invalid:
            // Leave it empty
            break;
        case NetVariantTypeEnum_String:
            *destinationString = source->getString();
            break;
        default:
            qWarning("Attempting to set a variant id %d to a QString", source->getVariantType());
            break;
        }
    }
    void unpack(QSharedPointer<NetVariant> destination, void* source, NetVariantTypeEnum)
    {
        QString* sourceString = static_cast<QString*>(source);
        destination->setString(sourceString);
    }
};

NetValueMetaObjectPacker::NetValueMetaObjectPacker()
{
    NetValueTypePacker* variantPacker = new NetValueTypePacker();
    StringValueTypePacker* stringPacker = new StringValueTypePacker();
    packers[NetVariantTypeEnum_Invalid] = variantPacker;
    packers[NetVariantTypeEnum_Bool] = variantPacker;
    packers[NetVariantTypeEnum_Char] = variantPacker;
    packers[NetVariantTypeEnum_Int] = variantPacker;
    packers[NetVariantTypeEnum_UInt] = variantPacker;
    packers[NetVariantTypeEnum_Double] = variantPacker;
    packers[NetVariantTypeEnum_String] = stringPacker;
    packers[NetVariantTypeEnum_DateTime] = variantPacker;
    packers[NetVariantTypeEnum_Object] = variantPacker;
    packers[NetVariantTypeEnum_JSValue] = variantPacker;
}

NetValueMetaObjectPacker* NetValueMetaObjectPacker::getInstance()
{
    static NetValueMetaObjectPacker packer;
    return &packer;
}

NetValueTypePacker* NetValueMetaObjectPacker::getPacker(NetVariantTypeEnum variantType)
{
    return packers[variantType];
}

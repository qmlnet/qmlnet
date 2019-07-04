#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetJsValue.h>
#include <QmlNet/qml/NetQObject.h>
#include <QDebug>

void NetValueTypePacker::pack(const QSharedPointer<NetVariant>& source, void* destination)
{
    QVariant* destinationVariant = static_cast<QVariant*>(destination);
    switch(source->getVariantType())
    {
    case NetVariantTypeEnum_Invalid:
        destinationVariant->clear();
        break;
    case NetVariantTypeEnum_Null:
        destinationVariant->setValue(nullptr);
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
    case NetVariantTypeEnum_ByteArray:
        destinationVariant->setValue(source->getBytes());
        break;
    case NetVariantTypeEnum_DateTime:
        destinationVariant->setValue(source->getDateTime());
        break;
    case NetVariantTypeEnum_Object: {
        QSharedPointer<NetReference> newInstance = source->getNetReference();
        NetValue* netValue = NetValue::forInstance(newInstance);
        destinationVariant->setValue(netValue);
        break;
    }
    case NetVariantTypeEnum_JSValue:
        destinationVariant->setValue(source->getJsValue()->getJsValue());
        break;
    case NetVariantTypeEnum_QObject:
        destinationVariant->setValue(source->getQObject()->getQObject());
        break;
    case NetVariantTypeEnum_NetVariantList:
        destinationVariant->clear();
        qWarning() << "TODO: Pack NetVariantList into a QVariantList";
        break;
    }
}

void NetValueTypePacker::unpack(const QSharedPointer<NetVariant>& destination, void* source)
{
    QVariant* sourceVariant = static_cast<QVariant*>(source);

    if(sourceVariant->isNull()) {
        destination->clear();
        return;
    }

    NetVariant::fromQVariant(sourceVariant, destination);
}

NetValueMetaObjectPacker::NetValueMetaObjectPacker()
{
    _packer = new NetValueTypePacker();
}

NetValueMetaObjectPacker* NetValueMetaObjectPacker::getInstance()
{
    static NetValueMetaObjectPacker packer;
    return &packer;
}

NetValueTypePacker* NetValueMetaObjectPacker::getPacker()
{
    return _packer;
}

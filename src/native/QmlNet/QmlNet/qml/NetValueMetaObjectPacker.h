#ifndef NETVALUEMETAOBJECTPACKER_H
#define NETVALUEMETAOBJECTPACKER_H

#include <QmlNet.h>
#include <QSharedPointer>
#include <QMap>

class NetVariant;

class NetValueTypePacker
{
public:
    NetValueTypePacker() {}
    virtual ~NetValueTypePacker() {}
    virtual void pack(const QSharedPointer<NetVariant>& source, void* destination);
    virtual void unpack(const QSharedPointer<NetVariant>& destination, void* source, NetVariantTypeEnum prefType);
};

#define NetMetaValuePack(type, source, destination) \
    NetValueMetaObjectPacker::getInstance()->getPacker(type)->pack(source, destination)
#define NetMetaValueUnpack(type, destination, source) \
    NetValueMetaObjectPacker::getInstance()->getPacker(type)->unpack(destination, source, type)

class NetValueMetaObjectPacker
{
public:
    NetValueMetaObjectPacker();
    static NetValueMetaObjectPacker* getInstance();
    NetValueTypePacker* getPacker(NetVariantTypeEnum variantType);
private:
    QMap<NetVariantTypeEnum, NetValueTypePacker*> packers;
};

#endif // NETVALUEMETAOBJECTPACKER_H

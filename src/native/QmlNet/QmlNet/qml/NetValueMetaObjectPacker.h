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
    virtual void unpack(const QSharedPointer<NetVariant>& destination, void* source);
};

#define NetMetaValuePack(source, destination) \
    NetValueMetaObjectPacker::getInstance()->getPacker()->pack(source, destination)
#define NetMetaValueUnpack(destination, source) \
    NetValueMetaObjectPacker::getInstance()->getPacker()->unpack(destination, source)

class NetValueMetaObjectPacker
{
public:
    NetValueMetaObjectPacker();
    static NetValueMetaObjectPacker* getInstance();
    NetValueTypePacker* getPacker();
private:
    NetValueTypePacker* _packer;
};

#endif // NETVALUEMETAOBJECTPACKER_H

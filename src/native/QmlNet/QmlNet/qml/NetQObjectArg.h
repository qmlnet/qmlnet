#ifndef NETQOBJECTARG_H
#define NETQOBJECTARG_H

#include <QMetaMethod>
#include <QSharedPointer>

class NetVariant;

class NetQObjectArg
{
public:
    NetQObjectArg();
    NetQObjectArg(int metaTypeId, QSharedPointer<NetVariant> netVariant = nullptr);
    ~NetQObjectArg();
    QGenericArgument genericArgument();
    QGenericReturnArgument genericReturnArguemnet();
    QSharedPointer<NetVariant> getNetVariant();
    void pack();
    void unpack();
private:
    int _metaTypeId;
    void* _data;
    int test;
    QVariant _variant;
    QSharedPointer<NetVariant> _netVariant;
};

#endif // NETQOBJECTARG_H

#ifndef NETTYPEARRAYFACADE_H
#define NETTYPEARRAYFACADE_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetTypeInfo;
class NetReference;
class NetVariant;

class NetTypeArrayFacade
{
public:
    NetTypeArrayFacade();
    virtual ~NetTypeArrayFacade() {}
    static QSharedPointer<NetTypeArrayFacade> fromType(const QSharedPointer<NetTypeInfo>& type);
    virtual bool isFixed();
    virtual uint getLength(const QSharedPointer<NetReference>& reference);
    virtual QSharedPointer<NetVariant> getIndexed(const QSharedPointer<NetReference>& reference, uint index);
    virtual void setIndexed(const QSharedPointer<NetReference>& reference, uint index, const QSharedPointer<NetVariant>& value);
    virtual void push(const QSharedPointer<NetReference>& reference, const QSharedPointer<NetVariant>& value);
    virtual QSharedPointer<NetVariant> pop(const QSharedPointer<NetReference>& reference);
    virtual void deleteAt(const QSharedPointer<NetReference>& reference, uint index);
};

#endif // NETTYPEARRAYFACADE_H

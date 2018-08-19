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
    static QSharedPointer<NetTypeArrayFacade> fromType(QSharedPointer<NetTypeInfo> type);
    virtual bool isFixed();
    virtual uint getLength(QSharedPointer<NetReference> reference);
    virtual QSharedPointer<NetVariant> getIndexed(QSharedPointer<NetReference> reference, uint index);
    virtual void setIndexed(QSharedPointer<NetReference> reference, uint index, QSharedPointer<NetVariant> value);
    virtual void push(QSharedPointer<NetReference> reference, QSharedPointer<NetVariant> value);
    virtual QSharedPointer<NetVariant> pop(QSharedPointer<NetReference> reference);
    virtual void deleteAt(QSharedPointer<NetReference> reference, uint index);
};

#endif // NETTYPEARRAYFACADE_H

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
    virtual uint getLength(QSharedPointer<NetReference> reference);
    virtual QSharedPointer<NetVariant> getIndexed(QSharedPointer<NetReference> reference, int index);
    virtual void setIndexed(QSharedPointer<NetReference> reference, int index, QSharedPointer<NetVariant> value);
};

#endif // NETTYPEARRAYFACADE_H

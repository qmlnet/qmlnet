#ifndef NETTYPEARRAYFACADE_H
#define NETTYPEARRAYFACADE_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetTypeInfo;
class NetReference;

class NetTypeArrayFacade
{
public:
    NetTypeArrayFacade();
    virtual ~NetTypeArrayFacade() {}
    static QSharedPointer<NetTypeArrayFacade> fromType(QSharedPointer<NetTypeInfo> type);
    virtual int getLength(QSharedPointer<NetReference> reference);
};

#endif // NETTYPEARRAYFACADE_H

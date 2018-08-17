#ifndef NETTYPEARRAYFACADEARRAY_H
#define NETTYPEARRAYFACADEARRAY_H

#include <QmlNet/types/NetTypeArrayFacade.h>

class NetMethodInfo;
class NetPropertyInfo;

class NetTypeArrayFacade_Array : public NetTypeArrayFacade
{
public:
    NetTypeArrayFacade_Array(QSharedPointer<NetTypeInfo> type);
    bool isIncomplete();
    int getLength(QSharedPointer<NetReference> reference);
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
};

#endif // NETTYPEARRAYFACADEARRAY_H

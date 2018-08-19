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
    bool isFixed();
    uint getLength(QSharedPointer<NetReference> reference);
    QSharedPointer<NetVariant> getIndexed(QSharedPointer<NetReference> reference, uint index);
    void setIndexed(QSharedPointer<NetReference> reference, uint index, QSharedPointer<NetVariant> value);
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
    QSharedPointer<NetMethodInfo> _getIndexed;
    QSharedPointer<NetMethodInfo> _setIndexed;
};

#endif // NETTYPEARRAYFACADEARRAY_H

#ifndef NETTYPEARRAYFACADELIST_H
#define NETTYPEARRAYFACADELIST_H

#include <QmlNet/types/NetTypeArrayFacade.h>

class NetMethodInfo;
class NetPropertyInfo;

class NetTypeArrayFacade_List : public NetTypeArrayFacade
{
public:
    NetTypeArrayFacade_List(QSharedPointer<NetTypeInfo> type);
    bool isIncomplete();
    bool isFixed();
    uint getLength(QSharedPointer<NetReference> reference);
    QSharedPointer<NetVariant> getIndexed(QSharedPointer<NetReference> reference, uint index);
    void setIndexed(QSharedPointer<NetReference> reference, uint index, QSharedPointer<NetVariant> value);
    QSharedPointer<NetVariant> pop(QSharedPointer<NetReference> reference);
    void deleteAt(QSharedPointer<NetReference> reference, uint index);
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
    QSharedPointer<NetPropertyInfo> _itemProperty;
    QSharedPointer<NetMethodInfo> _removeAtMethod;
};

#endif // NETTYPEARRAYFACADELIST_H

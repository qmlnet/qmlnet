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
    QSharedPointer<NetVariant> getIndexed(QSharedPointer<NetReference> reference, int index);
    void setIndexed(QSharedPointer<NetReference> reference, int index, QSharedPointer<NetVariant> value);
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
    QSharedPointer<NetPropertyInfo> _itemProperty;
};

#endif // NETTYPEARRAYFACADELIST_H

#ifndef NETTYPEARRAYFACADELIST_H
#define NETTYPEARRAYFACADELIST_H

#include <QmlNet/types/NetTypeArrayFacade.h>

class NetMethodInfo;
class NetPropertyInfo;

class NetTypeArrayFacade_List : public NetTypeArrayFacade
{
public:
    NetTypeArrayFacade_List(const QSharedPointer<NetTypeInfo>& type);
    bool isIncomplete();
    bool isFixed() override;
    uint getLength(const QSharedPointer<NetReference>& reference) override;
    QSharedPointer<NetVariant> getIndexed(const QSharedPointer<NetReference>& reference, uint index) override;
    void setIndexed(const QSharedPointer<NetReference>& reference, uint index, const QSharedPointer<NetVariant>& value) override;
    void push(const QSharedPointer<NetReference>& reference, const QSharedPointer<NetVariant>& value) override;
    QSharedPointer<NetVariant> pop(const QSharedPointer<NetReference>& reference) override;
    void deleteAt(const QSharedPointer<NetReference>& reference, uint index) override;
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
    QSharedPointer<NetPropertyInfo> _itemProperty;
    QSharedPointer<NetMethodInfo> _removeAtMethod;
    QSharedPointer<NetMethodInfo> _addMethod;
};

#endif // NETTYPEARRAYFACADELIST_H

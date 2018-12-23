#ifndef NETTYPEARRAYFACADEARRAY_H
#define NETTYPEARRAYFACADEARRAY_H

#include <QmlNet/types/NetTypeArrayFacade.h>

class NetMethodInfo;
class NetPropertyInfo;

class NetTypeArrayFacade_Array : public NetTypeArrayFacade
{
public:
    NetTypeArrayFacade_Array(const QSharedPointer<NetTypeInfo>& type);
    bool isIncomplete();
    bool isFixed() override;
    uint getLength(const QSharedPointer<NetReference>& reference) override;
    QSharedPointer<NetVariant> getIndexed(const QSharedPointer<NetReference>& reference, uint index) override;
    void setIndexed(const QSharedPointer<NetReference>& reference, uint index, const QSharedPointer<NetVariant>& value) override;
private:
    bool _isIncomplete;
    QSharedPointer<NetPropertyInfo> _lengthProperty;
    QSharedPointer<NetMethodInfo> _getIndexed;
    QSharedPointer<NetMethodInfo> _setIndexed;
};

#endif // NETTYPEARRAYFACADEARRAY_H

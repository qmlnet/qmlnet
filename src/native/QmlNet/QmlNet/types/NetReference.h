#ifndef NetReference_H
#define NetReference_H

#include <QmlNet/types/NetTypeInfo.h>

class NetReference
{
public:
    NetReference(uint64_t objectId, QSharedPointer<NetTypeInfo> typeInfo);
    NetReference(uint64_t objectId, int aotTypeId);
    ~NetReference();
    uint64_t getObjectId();
    QSharedPointer<NetTypeInfo> getTypeInfo();
    int aotTypeId();
    bool isAot();
    QString displayName();
private:
    uint64_t _objectId;
    QSharedPointer<NetTypeInfo> _typeInfo;
    int _aotTypeId;
    bool _isAot;
};

struct NetReferenceContainer {
    QSharedPointer<NetReference> instance;
};

#endif // NetReference_H

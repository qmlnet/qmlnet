#ifndef NetReference_H
#define NetReference_H

#include <QmlNet/types/NetTypeInfo.h>

class NetReference
{
public:
    NetReference(uint64_t objectId, QSharedPointer<NetTypeInfo> typeInfo);
    ~NetReference();
    uint64_t getObjectId();
    QSharedPointer<NetTypeInfo> getTypeInfo();
private:
    uint64_t objectId;
    QSharedPointer<NetTypeInfo> typeInfo;
};

struct NetReferenceContainer {
    QSharedPointer<NetReference> instance;
};

#endif // NetReference_H

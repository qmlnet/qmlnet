#ifndef NetReference_H
#define NetReference_H

#include <QmlNet/types/NetTypeInfo.h>

class NetReference
{
public:
    NetReference(NetGCHandle* gcHandle, uint64_t objectId, QSharedPointer<NetTypeInfo> typeInfo);
    ~NetReference();
    NetGCHandle* getGCHandle();
    uint64_t getObjectId();
    QSharedPointer<NetTypeInfo> getTypeInfo();

    void release();
private:
    NetGCHandle* gcHandle;
    uint64_t objectId;
    QSharedPointer<NetTypeInfo> typeInfo;
};

struct NetReferenceContainer {
    QSharedPointer<NetReference> instance;
};

#endif // NetReference_H

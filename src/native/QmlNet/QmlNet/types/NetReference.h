#ifndef NetReference_H
#define NetReference_H

#include <QmlNet/types/NetTypeInfo.h>

class NetReference
{
public:
    NetReference(NetGCHandle* gcHandle, QSharedPointer<NetTypeInfo> typeInfo);
    ~NetReference();
    NetGCHandle* getGCHandle();
    QSharedPointer<NetTypeInfo> getTypeInfo();

    void release();
private:
    NetGCHandle* gcHandle;
    QSharedPointer<NetTypeInfo> typeInfo;
};

struct NetReferenceContainer {
    QSharedPointer<NetReference> instance;
};

#endif // NetReference_H

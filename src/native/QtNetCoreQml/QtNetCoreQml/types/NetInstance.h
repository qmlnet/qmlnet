#ifndef NETINSTANCE_H
#define NETINSTANCE_H

#include <QtNetCoreQml/types/NetTypeInfo.h>

class NetInstance
{
public:
    NetInstance(NetGCHandle* gcHandle, QSharedPointer<NetTypeInfo> typeInfo);
    ~NetInstance();
    NetGCHandle* getGCHandle();
    QSharedPointer<NetTypeInfo> getTypeInfo();
private:
    NetGCHandle* gcHandle;
    QSharedPointer<NetTypeInfo> typeInfo;
};

struct NetInstanceContainer {
    QSharedPointer<NetInstance> instance;
};

#endif // NETINSTANCE_H

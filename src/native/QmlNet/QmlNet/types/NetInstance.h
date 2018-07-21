#ifndef NETINSTANCE_H
#define NETINSTANCE_H

#include <QmlNet/types/NetTypeInfo.h>

class NetInstance
{
public:
    NetInstance(NetGCHandle* gcHandle, QSharedPointer<NetTypeInfo> typeInfo);
    ~NetInstance();
    NetGCHandle* getGCHandle();
    QSharedPointer<NetTypeInfo> getTypeInfo();

    void release();
private:
    NetGCHandle* gcHandle;
    QSharedPointer<NetTypeInfo> typeInfo;
};

struct NetInstanceContainer {
    QSharedPointer<NetInstance> instance;
};

#endif // NETINSTANCE_H

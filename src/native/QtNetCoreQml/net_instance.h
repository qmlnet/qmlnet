#ifndef NET_INSTANCE_H
#define NET_INSTANCE_H

#include "qtnetcoreqml_global.h"
#include <QVariant>
#include "net_type_info.h"

class NetInstance
{
public:
    NetInstance(NetGCHandle* gcHandle, NetTypeInfo* typeInfo);
    ~NetInstance();
    NetGCHandle* GetGCHandle();
    NetTypeInfo* GetTypeInfo();
private:
    NetGCHandle* gcHandle;
    NetTypeInfo* typeInfo;
};

#endif // NET_INSTANCE_H

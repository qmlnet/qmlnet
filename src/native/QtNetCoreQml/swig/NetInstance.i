%{
#include "net_instance.h"
%}

class NetInstance
{
public:
    NetInstance(NetGCHandle* gcHandle, NetTypeInfo* typeInfo);
    NetGCHandle* GetGCHandle();
    NetTypeInfo* GetTypeInfo();
    NetInstance* Clone();
};
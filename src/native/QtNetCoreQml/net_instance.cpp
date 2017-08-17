#include "net_instance.h"
#include "net_type_info_manager.h"

NetInstance::NetInstance(NetGCHandle* gcHandle, NetTypeInfo* typeInfo) :
    gcHandle(gcHandle),
    typeInfo(typeInfo)
{

}

NetInstance::~NetInstance()
{
    NetTypeInfoManager::ReleaseGCHandle(gcHandle);
    gcHandle = NULL;
    typeInfo = NULL;
}

NetGCHandle* NetInstance::GetGCHandle()
{
    return gcHandle;
}

NetTypeInfo* NetInstance::GetTypeInfo()
{
    return typeInfo;
}

NetInstance* NetInstance::Clone()
{
    return new NetInstance(NetTypeInfoManager::CopyGCHandle(gcHandle), typeInfo);
}

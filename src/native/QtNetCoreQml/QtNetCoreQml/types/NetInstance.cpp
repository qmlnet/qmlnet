#include <QtNetCoreQml/types/NetInstance.h>
#include <QtNetCoreQml/types/Callbacks.h>

NetInstance::NetInstance(NetGCHandle* gcHandle, QSharedPointer<NetTypeInfo> typeInfo) :
    gcHandle(gcHandle),
    typeInfo(typeInfo)
{

}

NetInstance::~NetInstance()
{
    releaseGCHandle(gcHandle);
    gcHandle = NULL;
    typeInfo = NULL;
}

NetGCHandle* NetInstance::getGCHandle()
{
    return gcHandle;
}

QSharedPointer<NetTypeInfo> NetInstance::getTypeInfo()
{
    return typeInfo;
}

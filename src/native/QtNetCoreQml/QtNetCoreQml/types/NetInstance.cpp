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

extern "C" {

NetInstanceContainer* net_instance_create(NetGCHandle* handle, NetTypeInfoContainer* typeContainer) {
    NetInstanceContainer* result = new NetInstanceContainer();
    result->instance = QSharedPointer<NetInstance>(new NetInstance(handle, typeContainer->netTypeInfo));
    return result;
}

void net_instance_destroy(NetInstanceContainer* container) {
    delete container;
}

NetGCHandle* net_instance_getHandle(NetInstanceContainer* container) {
    return container->instance->getGCHandle();
}

}

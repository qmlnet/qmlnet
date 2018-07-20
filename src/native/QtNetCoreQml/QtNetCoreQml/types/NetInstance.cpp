#include <QtNetCoreQml/types/NetInstance.h>
#include <QtNetCoreQml/types/Callbacks.h>
#include <QtNetCoreQml/qml/NetValue.h>
#include <QDebug>

NetInstance::NetInstance(NetGCHandle* gcHandle, QSharedPointer<NetTypeInfo> typeInfo) :
    gcHandle(gcHandle),
    typeInfo(typeInfo)
{
    qDebug("NetInstance created: %s", qPrintable(typeInfo->getClassName()));
}

NetInstance::~NetInstance()
{
    release();
}

NetGCHandle* NetInstance::getGCHandle()
{
    return gcHandle;
}

QSharedPointer<NetTypeInfo> NetInstance::getTypeInfo()
{
    return typeInfo;
}

void NetInstance::release()
{
    if(gcHandle != nullptr) {
        releaseGCHandle(gcHandle);
        auto typeInfoClassName = typeInfo->getClassName();
        gcHandle = nullptr;
        typeInfo = nullptr;
        qDebug("NetInstance released: %s", qPrintable(typeInfoClassName));
    }
}

extern "C" {

Q_DECL_EXPORT NetInstanceContainer* net_instance_create(NetGCHandle* handle, NetTypeInfoContainer* typeContainer) {
    NetInstanceContainer* result = new NetInstanceContainer();
    result->instance = QSharedPointer<NetInstance>(new NetInstance(handle, typeContainer->netTypeInfo));
    return result;
}

Q_DECL_EXPORT void net_instance_destroy(NetInstanceContainer* container) {
    delete container;
}

Q_DECL_EXPORT NetInstanceContainer* net_instance_clone(NetInstanceContainer* container) {
    NetInstanceContainer* result = new NetInstanceContainer{container->instance};
    return result;
}

Q_DECL_EXPORT NetGCHandle* net_instance_getHandle(NetInstanceContainer* container) {
    return container->instance->getGCHandle();
}

Q_DECL_EXPORT bool net_instance_activateSignal(NetInstanceContainer* container, LPWSTR signalName, NetVariantListContainer* parametersContainer) {
    NetValue* existing = NetValue::forInstance(container->instance);
    if(existing == NULL) {
        // Not alive in the QML world, so no signals to raise.
        return false;
    }
    QString signalNameString = QString::fromUtf16((const char16_t*)signalName);

    QSharedPointer<NetVariantList> parameters;
    if(parametersContainer != NULL) {
        parameters = parametersContainer->list;
    }

    return existing->activateSignal(signalNameString, parameters);
}

}

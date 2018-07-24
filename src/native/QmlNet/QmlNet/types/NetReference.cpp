#include <QmlNet/types/NetReference.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/qml/NetValue.h>
#include <QDebug>

NetReference::NetReference(uint64_t objectId, QSharedPointer<NetTypeInfo> typeInfo) :
    objectId(objectId),
    typeInfo(typeInfo)
{
}

NetReference::~NetReference()
{
    release();
}

uint64_t NetReference::getObjectId()
{
    return objectId;
}

QSharedPointer<NetTypeInfo> NetReference::getTypeInfo()
{
    return typeInfo;
}

void NetReference::release()
{
    if(typeInfo != nullptr) {
        releaseNetReference(objectId);
        typeInfo = nullptr;
    }
}

extern "C" {

Q_DECL_EXPORT NetReferenceContainer* net_instance_create(uint64_t objectId, NetTypeInfoContainer* typeContainer) {
    NetReferenceContainer* result = new NetReferenceContainer();
    result->instance = QSharedPointer<NetReference>(new NetReference(objectId, typeContainer->netTypeInfo));
    return result;
}

Q_DECL_EXPORT void net_instance_destroy(NetReferenceContainer* container) {
    delete container;
}

Q_DECL_EXPORT NetReferenceContainer* net_instance_clone(NetReferenceContainer* container) {
    NetReferenceContainer* result = new NetReferenceContainer{container->instance};
    return result;
}

Q_DECL_EXPORT uint64_t net_instance_getObjectId(NetReferenceContainer* container) {
    return container->instance->getObjectId();
}

Q_DECL_EXPORT bool net_instance_activateSignal(NetReferenceContainer* container, LPWSTR signalName, NetVariantListContainer* parametersContainer) {
    NetValue* existing = NetValue::forInstance(container->instance, false);
    if(existing == nullptr) {
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

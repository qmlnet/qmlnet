#include <QmlNet/types/NetReference.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/qml/NetValue.h>
#include <QDebug>
#include <utility>

NetReference::NetReference(uint64_t objectId, QSharedPointer<NetTypeInfo> typeInfo) :
    _objectId(objectId),
    _typeInfo(std::move(typeInfo)),
    _aotTypeId(-1),
    _isAot(false)
{
}

NetReference::NetReference(uint64_t objectId, int aotTypeId) :
    _objectId(objectId),
    _aotTypeId(aotTypeId),
    _isAot(true)
{

}

NetReference::~NetReference()
{
    QmlNet::releaseNetReference(_objectId);
}

uint64_t NetReference::getObjectId()
{
    return _objectId;
}

QSharedPointer<NetTypeInfo> NetReference::getTypeInfo()
{
    return _typeInfo;
}

int NetReference::aotTypeId()
{
    return _aotTypeId;
}

bool NetReference::isAot()
{
    return _isAot;
}

QString NetReference::displayName()
{
    QString result = _typeInfo->getClassName();
    result.append("(");
    result.append(QVariant::fromValue(_objectId).toString());
    result.append(")");
    return result;
}

extern "C" {

Q_DECL_EXPORT NetReferenceContainer* net_instance_create(uint64_t objectId, NetTypeInfoContainer* typeContainer) {
    NetReferenceContainer* result = new NetReferenceContainer();
    result->instance = QSharedPointer<NetReference>(new NetReference(objectId, typeContainer->netTypeInfo));
    return result;
}

Q_DECL_EXPORT NetReferenceContainer* net_instance_createAot(uint64_t objectId, int aotTypeId) {
    NetReferenceContainer* result = new NetReferenceContainer();
    result->instance = QSharedPointer<NetReference>(new NetReference(objectId, aotTypeId));
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

Q_DECL_EXPORT uchar net_instance_isAot(NetReferenceContainer* container) {
    return container->instance->isAot() ? 1 : 0;
}



Q_DECL_EXPORT uchar net_instance_activateSignal(NetReferenceContainer* container, LPWCSTR signalName, NetVariantListContainer* parametersContainer) {
    QList<NetValue*> liveInstances = NetValue::getAllLiveInstances(container->instance);
    if(liveInstances.length() == 0) {
        // Not alive in the QML world, so no signals to raise
        return false;
    }

    QString signalNameString = QString::fromUtf16(signalName);

    QSharedPointer<NetVariantList> parameters;
    if(parametersContainer != nullptr) {
        parameters = parametersContainer->list;
    }

    bool result = false;
    for(NetValue* liveInstance : liveInstances) {
        result = result || liveInstance->activateSignal(signalNameString, parameters);
    }

    if(result) {
        return 1;
    } else {
        return 0;
    }
}

}

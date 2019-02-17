#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QmlNetUtilities.h>
#include <QMutex>
#include <utility>

static int nextPropertyId = 1;
Q_GLOBAL_STATIC(QMutex, propertyIdMutex);

NetPropertyInfo::NetPropertyInfo(QSharedPointer<NetTypeInfo> parentType,
        QString name,
        QSharedPointer<NetTypeInfo> returnType,
        bool canRead,
        bool canWrite,
        QSharedPointer<NetSignalInfo> notifySignal) :
    _parentType(std::move(parentType)),
    _name(std::move(name)),
    _returnType(std::move(returnType)),
    _canRead(canRead),
    _canWrite(canWrite),
    _notifySignal(std::move(notifySignal))
{
    propertyIdMutex->lock();
    _id = nextPropertyId;
    nextPropertyId++;
    propertyIdMutex->unlock();
}

int NetPropertyInfo::getId()
{
    return _id;
}

QSharedPointer<NetTypeInfo> NetPropertyInfo::getParentType()
{
    return _parentType;
}

QString NetPropertyInfo::getPropertyName()
{
    return _name;
}

QSharedPointer<NetTypeInfo> NetPropertyInfo::getReturnType()
{
    return _returnType;
}

bool NetPropertyInfo::canRead()
{
    return _canRead;
}

bool NetPropertyInfo::canWrite()
{
    return _canWrite;
}

QSharedPointer<NetSignalInfo> NetPropertyInfo::getNotifySignal()
{
    return _notifySignal;
}

void NetPropertyInfo::setNotifySignal(QSharedPointer<NetSignalInfo> signal)
{
    _notifySignal = std::move(signal);
}

extern "C" {

Q_DECL_EXPORT NetPropertyInfoContainer* property_info_create(NetTypeInfoContainer* parentTypeContainer,
                                               LPWSTR name,
                                               NetTypeInfoContainer* returnType,
                                               uchar canRead,
                                               uchar canWrite,
                                               NetSignalInfoContainer* notifySignalContainer) {
    NetPropertyInfoContainer* result = new NetPropertyInfoContainer();
    QSharedPointer<NetSignalInfo> notifySignal;
    if(notifySignalContainer != nullptr) {
        notifySignal = notifySignalContainer->signal;
    }
    NetPropertyInfo* instance = new NetPropertyInfo(parentTypeContainer->netTypeInfo,
                                                    QString::fromUtf16(static_cast<const char16_t*>(name)),
                                                    returnType->netTypeInfo,
                                                    canRead == 1 ? true  : false,
                                                    canWrite == 1 ? true : false,
                                                    notifySignal);
    result->property = QSharedPointer<NetPropertyInfo>(instance);
    return result;
}

Q_DECL_EXPORT void property_info_destroy(NetTypeInfoContainer* container) {
    delete container;
}

Q_DECL_EXPORT int property_info_getId(NetPropertyInfoContainer* container)
{
    return container->property->getId();
}

Q_DECL_EXPORT NetTypeInfoContainer* property_info_getParentType(NetPropertyInfoContainer* container) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->property->getParentType();
    return result;
}


Q_DECL_EXPORT QmlNetStringContainer* property_info_getPropertyName(NetPropertyInfoContainer* container) {
    QString result = container->property->getPropertyName();
    return createString(result);
}

Q_DECL_EXPORT NetTypeInfoContainer* property_info_getReturnType(NetPropertyInfoContainer* container) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->property->getReturnType();
    return result;
}

Q_DECL_EXPORT uchar property_info_canRead(NetPropertyInfoContainer* container) {
    if(container->property->canRead()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT uchar property_info_canWrite(NetPropertyInfoContainer* container) {
    if(container->property->canWrite()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT NetSignalInfoContainer* property_info_getNotifySignal(NetPropertyInfoContainer* container) {
    QSharedPointer<NetSignalInfo> notifySignal = container->property->getNotifySignal();
    if(notifySignal == nullptr) {
        return nullptr;
    }
    return new NetSignalInfoContainer{notifySignal};
}

Q_DECL_EXPORT void property_info_setNotifySignal(NetPropertyInfoContainer* container, NetSignalInfoContainer* signalContainer) {
    container->property->setNotifySignal(signalContainer->signal);
}



}

#include <QmlNet/types/NetDelegate.h>
#include <QmlNet/types/Callbacks.h>

NetDelegate::NetDelegate(NetGCHandle* gcHandle) :
    gcHandle(gcHandle) {

}

NetDelegate::~NetDelegate() {
    QmlNet::releaseNetDelegateGCHandle(gcHandle);
}

NetGCHandle* NetDelegate::getGCHandle() {
    return gcHandle;
}

extern "C" {

Q_DECL_EXPORT NetDelegateContainer* delegate_create(NetGCHandle* gcHandle) {
    NetDelegateContainer* result = new NetDelegateContainer();
    result->delegate = QSharedPointer<NetDelegate>(new NetDelegate(gcHandle));
    return result;
}

Q_DECL_EXPORT void delegate_destroy(NetDelegateContainer* container) {
    delete container;
}

Q_DECL_EXPORT NetGCHandle* delegate_getHandle(NetDelegateContainer* container) {
    return container->delegate->getGCHandle();
}

}

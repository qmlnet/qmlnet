#include <QtNetCoreQml/types/Callbacks.h>
#include <iostream>

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*buildTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseGCHandleCb)(NetGCHandle* handle);
typedef NetInstanceContainer* (*instantiateTypeCb)(NetTypeInfoContainer* typeInfo);

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

struct NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    buildTypeInfoCb buildTypeInfo;
    releaseGCHandleCb releaseGCHandle;
    instantiateTypeCb instantiateType;
};

static NetTypeInfoManagerCallbacks sharedCallbacks;

bool isTypeValid(QString type) {
    return sharedCallbacks.isTypeValid((LPWSTR)type.utf16());
}

void releaseGCHandle(NetGCHandle* handle) {
    return sharedCallbacks.releaseGCHandle(handle);
}

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    sharedCallbacks.buildTypeInfo(container);
}

QSharedPointer<NetInstance> instantiateType(QSharedPointer<NetTypeInfo> type) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = type;
    NetInstanceContainer* result = sharedCallbacks.instantiateType(container);
    if(result == NULL) {
        return QSharedPointer<NetInstance>(NULL);
    } else {
        return result->instance;
    }
}

extern "C" {

void type_info_callbacks_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

bool type_info_callbacks_isTypeValid(LPWSTR typeName) {
    return sharedCallbacks.isTypeValid(typeName);
}

void type_info_callbacks_releaseGCHandle(NetGCHandle* handle) {
    sharedCallbacks.releaseGCHandle(handle);
}

void type_info_callbacks_buildTypeInfo(NetTypeInfoContainer* typeInfo) {
    sharedCallbacks.buildTypeInfo(typeInfo);
}

NetInstanceContainer* type_info_callbacks_instantiateType(NetTypeInfoContainer* typeInfo) {
    return sharedCallbacks.instantiateType(typeInfo);
}

}

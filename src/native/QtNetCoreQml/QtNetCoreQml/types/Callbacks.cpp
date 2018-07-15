#include <QtNetCoreQml/types/Callbacks.h>
#include <QtNetCoreQml/types/NetPropertyInfo.h>
#include <QtNetCoreQml/qml/NetVariant.h>
#include <iostream>

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*buildTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseGCHandleCb)(NetGCHandle* handle);
typedef NetGCHandle* (*instantiateTypeCb)(LPWSTR typeName);
typedef void (*readPropertyCb)(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* result);
typedef void (*writePropertyCb)(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* value);

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

struct NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    buildTypeInfoCb buildTypeInfo;
    releaseGCHandleCb releaseGCHandle;
    instantiateTypeCb instantiateType;
    readPropertyCb readProperty;
    writePropertyCb writeProperty;
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
    NetGCHandle* result = sharedCallbacks.instantiateType((LPWSTR)type->getFullTypeName().utf16());
    if(result == NULL) {
        return QSharedPointer<NetInstance>(NULL);
    } else {
        return QSharedPointer<NetInstance>(new NetInstance(result, type));
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

NetGCHandle* type_info_callbacks_instantiateType(LPWSTR typeName) {
    return sharedCallbacks.instantiateType(typeName);
}

void type_info_callbacks_readProperty(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* result) {
    //sharedCallbacks.readProperty(property, target, result);
}

void type_info_callbacks_writeProperty(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* value) {
    //sharedCallbacks.writeProperty(property, target, value);
}

}

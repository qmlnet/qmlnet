#include <QmlNet/types/Callbacks.h>

namespace QmlNet {

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*createLazyTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*loadTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseNetReferenceCb)(uint64_t objectId);
typedef void (*releaseNetDelegateGCHandleCb)(NetGCHandle* handle);
typedef NetReferenceContainer* (*instantiateTypeCb)(NetTypeInfoContainer* type);
typedef void (*readPropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* indexParameter, NetVariantContainer* result);
typedef void (*writePropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* indexParameter, NetVariantContainer* value);
typedef void (*invokeMethodCb)(NetMethodInfoContainer* method, NetReferenceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result);
typedef void (*gcCollectCb)(int maxGeneration);
typedef bool (*raiseNetSignalsCb)(NetReferenceContainer* target, LPWCSTR signalName, NetVariantListContainer* parameters);
typedef void (*awaitTaskCb)(NetReferenceContainer* target, NetJSValueContainer* successCallback, NetJSValueContainer* failureCallback);
typedef bool (*serializeNetToStringCb)(NetReferenceContainer* instance, NetVariantContainer* result);

struct Q_DECL_EXPORT NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    createLazyTypeInfoCb createLazyTypeInfo;
    loadTypeInfoCb loadTypeInfo;
    releaseNetReferenceCb releaseNetReference;
    releaseNetDelegateGCHandleCb releaseNetDelegateGCHandle;
    instantiateTypeCb instantiateType;
    readPropertyCb readProperty;
    writePropertyCb writeProperty;
    invokeMethodCb invokeMethod;
    gcCollectCb gcCollect;
    raiseNetSignalsCb raiseNetSignals;
    awaitTaskCb awaitTask;
    serializeNetToStringCb serializeNetToString;
};

static NetTypeInfoManagerCallbacks sharedCallbacks;

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

bool isTypeValid(QString type) {
    return sharedCallbacks.isTypeValid((LPWSTR)type.utf16());
}

void releaseNetReference(uint64_t objectId) {
    return sharedCallbacks.releaseNetReference(objectId);
}

void releaseNetDelegateGCHandle(NetGCHandle* handle) {
    return sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

void createLazyTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    sharedCallbacks.createLazyTypeInfo(container);
}

void loadTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    sharedCallbacks.loadTypeInfo(container);
}

QSharedPointer<NetReference> instantiateType(QSharedPointer<NetTypeInfo> type) {
    NetTypeInfoContainer* typeContainer = new NetTypeInfoContainer{ type }; // .NET will delete this type
    NetReferenceContainer* resultContainer = sharedCallbacks.instantiateType(typeContainer);

    QSharedPointer<NetReference> result;

    if(resultContainer != nullptr) {
        result = resultContainer->instance;
        // Special care is given here. .NET
        // has given us a container that it will NOT delete itself.
        // This means that .NET didn't wrap the pointer up in an object
        // that will be GC'd.
        delete resultContainer;
    }

    return result;
}

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> indexParameter, QSharedPointer<NetVariant> result) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = target;
    NetVariantContainer* indexParameterContainer = nullptr;
    if(indexParameter != nullptr) {
        indexParameterContainer = new NetVariantContainer{indexParameter};
    }
    NetVariantContainer* valueContainer = new NetVariantContainer();
    valueContainer->variant = result;
    sharedCallbacks.readProperty(propertyContainer, targetContainer, indexParameterContainer, valueContainer);
    // The callbacks dispose of the types.
}

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> indexParameter, QSharedPointer<NetVariant> value) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = target;
    NetVariantContainer* indexParameterContainer = nullptr;
    if(indexParameter != nullptr) {
        indexParameterContainer = new NetVariantContainer{indexParameter};
    }
    NetVariantContainer* resultContainer = new NetVariantContainer();
    resultContainer->variant = value;
    sharedCallbacks.writeProperty(propertyContainer, targetContainer, indexParameterContainer, resultContainer);
    // The callbacks dispose of the types.
}

void invokeNetMethod(QSharedPointer<NetMethodInfo> method, QSharedPointer<NetReference> target, QSharedPointer<NetVariantList> parameters, QSharedPointer<NetVariant> result) {
    NetMethodInfoContainer* methodContainer = new NetMethodInfoContainer();
    methodContainer->method = method;
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = target;
    NetVariantListContainer* parametersContainer = new NetVariantListContainer();
    parametersContainer->list = parameters;
    NetVariantContainer* resultContainer = nullptr;
    if(result != nullptr) {
        // There is a return type.
        resultContainer = new NetVariantContainer();
        resultContainer->variant = result;
    }
    sharedCallbacks.invokeMethod(methodContainer, targetContainer, parametersContainer, resultContainer);
    // The callbacks dispose of types.
}

void gcCollect(int maxGeneration) {
    sharedCallbacks.gcCollect(maxGeneration);
}

bool raiseNetSignals(QSharedPointer<NetReference> target, QString signalName, QSharedPointer<NetVariantList> parameters) {
    NetReferenceContainer* targetContainer = new NetReferenceContainer{target};
    NetVariantListContainer* parametersContainer = nullptr;
    if(parameters != nullptr) {
        parametersContainer = new NetVariantListContainer{parameters};
    }
    return sharedCallbacks.raiseNetSignals(targetContainer, (LPWCSTR)signalName.utf16(), parametersContainer);
}

void awaitTask(QSharedPointer<NetReference> target, QSharedPointer<NetJSValue> successCallback, QSharedPointer<NetJSValue> failureCallback) {
    NetReferenceContainer* targetContainer = new NetReferenceContainer{target};
    NetJSValueContainer* sucessCallbackContainer = new NetJSValueContainer{successCallback};
    NetJSValueContainer* failureCallbackContainer = nullptr;
    if(failureCallback != nullptr) {
        failureCallbackContainer = new NetJSValueContainer{failureCallback};
    }
    sharedCallbacks.awaitTask(targetContainer, sucessCallbackContainer, failureCallbackContainer);
}

bool serializeNetToString(QSharedPointer<NetReference> instance, QSharedPointer<NetVariant> result)
{
    return sharedCallbacks.serializeNetToString(new NetReferenceContainer{instance},
                                                new NetVariantContainer{result});
}

}

extern "C" {

Q_DECL_EXPORT void type_info_callbacks_registerCallbacks(QmlNet::NetTypeInfoManagerCallbacks* callbacks) {
    QmlNet::sharedCallbacks = *callbacks;
}

Q_DECL_EXPORT bool type_info_callbacks_isTypeValid(LPWSTR typeName) {
    return QmlNet::sharedCallbacks.isTypeValid(typeName);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetReferenceGCHandle(uint64_t objectId) {
    QmlNet::sharedCallbacks.releaseNetReference(objectId);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetDelegateGCHandle(NetGCHandle* handle) {
    QmlNet::sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

Q_DECL_EXPORT NetReferenceContainer* type_info_callbacks_instantiateType(NetTypeInfoContainer* type) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetTypeInfoContainer* typeCopy = new NetTypeInfoContainer{type->netTypeInfo};
    return QmlNet::sharedCallbacks.instantiateType(typeCopy);
}

Q_DECL_EXPORT void type_info_callbacks_invokeMethod(NetMethodInfoContainer* method, NetReferenceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.

    NetMethodInfoContainer* methodCopy = new NetMethodInfoContainer{method->method};
    NetReferenceContainer* targetCopy = new NetReferenceContainer{target->instance};
    NetVariantListContainer* parametersCopy = nullptr;
    NetVariantContainer* resultCopy = nullptr;

    if(parameters != nullptr) {
        parametersCopy = new NetVariantListContainer{parameters->list};
    }
    if(result != nullptr) {
        resultCopy = new NetVariantContainer{result->variant};
    }

    QmlNet::sharedCallbacks.invokeMethod(
                methodCopy,
                targetCopy,
                parametersCopy,
                resultCopy);
}

}

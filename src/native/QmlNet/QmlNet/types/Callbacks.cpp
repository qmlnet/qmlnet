#include <QmlNet/types/Callbacks.h>

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*createLazyTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*loadTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseNetReferenceCb)(uint64_t objectId);
typedef void (*releaseNetDelegateGCHandleCb)(NetGCHandle* handle);
typedef NetReferenceContainer* (*instantiateTypeCb)(NetTypeInfoContainer* type);
typedef void (*readPropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* result);
typedef void (*writePropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* value);
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

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> result) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = target;
    NetVariantContainer* valueContainer = new NetVariantContainer();
    valueContainer->variant = result;
    sharedCallbacks.readProperty(propertyContainer, targetContainer, valueContainer);
    // The callbacks dispose of the types.
}

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> value) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = target;
    NetVariantContainer* resultContainer = new NetVariantContainer();
    resultContainer->variant = value;
    sharedCallbacks.writeProperty(propertyContainer, targetContainer, resultContainer);
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

extern "C" {

Q_DECL_EXPORT void type_info_callbacks_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

Q_DECL_EXPORT bool type_info_callbacks_isTypeValid(LPWSTR typeName) {
    return sharedCallbacks.isTypeValid(typeName);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetReferenceGCHandle(uint64_t objectId) {
    sharedCallbacks.releaseNetReference(objectId);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetDelegateGCHandle(NetGCHandle* handle) {
    sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

Q_DECL_EXPORT NetReferenceContainer* type_info_callbacks_instantiateType(NetTypeInfoContainer* type) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetTypeInfoContainer* typeCopy = new NetTypeInfoContainer{type->netTypeInfo};
    return sharedCallbacks.instantiateType(typeCopy);
}

Q_DECL_EXPORT void type_info_callbacks_readProperty(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* result) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetPropertyInfoContainer* propertyCopy = new NetPropertyInfoContainer{property->property};
    NetReferenceContainer* targetCopy = new NetReferenceContainer{target->instance};
    NetVariantContainer* resultCopy = nullptr;

    if(result != nullptr) {
        resultCopy = new NetVariantContainer{result->variant};
    }

    sharedCallbacks.readProperty(propertyCopy, targetCopy, resultCopy);
}

Q_DECL_EXPORT void type_info_callbacks_writeProperty(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* value) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetPropertyInfoContainer* propertyCopy = new NetPropertyInfoContainer{property->property};
    NetReferenceContainer* targetCopy = new NetReferenceContainer{target->instance};
    NetVariantContainer* valueCopy = nullptr;

    if(value != nullptr) {
        valueCopy = new NetVariantContainer{value->variant};
    }

    sharedCallbacks.writeProperty(propertyCopy, targetCopy, valueCopy);
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

    sharedCallbacks.invokeMethod(
                methodCopy,
                targetCopy,
                parametersCopy,
                resultCopy);
}

}

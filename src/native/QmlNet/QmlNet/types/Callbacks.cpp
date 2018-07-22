#include <QmlNet/types/Callbacks.h>

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*buildTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseNetReferenceGCHandleCb)(NetGCHandle* handle, uint64_t objectId);
typedef void (*releaseNetDelegateGCHandleCb)(NetGCHandle* handle);
typedef NetReferenceContainer* (*instantiateTypeCb)(NetTypeInfoContainer* type);
typedef void (*readPropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* result);
typedef void (*writePropertyCb)(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* value);
typedef void (*invokeMethodCb)(NetMethodInfoContainer* method, NetReferenceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result);
typedef void (*gcCollectCb)(int maxGeneration);
typedef bool (*raiseNetSignalsCb)(NetReferenceContainer* target, LPWCSTR signalName, NetVariantListContainer* parameters);

struct Q_DECL_EXPORT NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    buildTypeInfoCb buildTypeInfo;
    releaseNetReferenceGCHandleCb releaseNetReferenceGCHandle;
    releaseNetDelegateGCHandleCb releaseNetDelegateGCHandle;
    instantiateTypeCb instantiateType;
    readPropertyCb readProperty;
    writePropertyCb writeProperty;
    invokeMethodCb invokeMethod;
    gcCollectCb gcCollect;
    raiseNetSignalsCb raiseNetSignals;
};

static NetTypeInfoManagerCallbacks sharedCallbacks;

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

bool isTypeValid(QString type) {
    return sharedCallbacks.isTypeValid((LPWSTR)type.utf16());
}

void releaseNetReferenceGCHandle(NetGCHandle* handle, uint64_t objectId) {
    return sharedCallbacks.releaseNetReferenceGCHandle(handle, objectId);
}

void releaseNetDelegateGCHandle(NetGCHandle* handle) {
    return sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    sharedCallbacks.buildTypeInfo(container);
}

QSharedPointer<NetReference> instantiateType(QSharedPointer<NetTypeInfo> type) {
    NetTypeInfoContainer* typeContainer = new NetTypeInfoContainer{ type }; // .NET will delete this type
    NetReferenceContainer* resultContainer = sharedCallbacks.instantiateType(typeContainer);

    QSharedPointer<NetReference> result;

    if(resultContainer != NULL) {
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

extern "C" {

Q_DECL_EXPORT void type_info_callbacks_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

Q_DECL_EXPORT bool type_info_callbacks_isTypeValid(LPWSTR typeName) {
    return sharedCallbacks.isTypeValid(typeName);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetReferenceGCHandle(NetGCHandle* handle, uint64_t objectId) {
    sharedCallbacks.releaseNetReferenceGCHandle(handle, objectId);
}

Q_DECL_EXPORT void type_info_callbacks_releaseNetDelegateGCHandle(NetGCHandle* handle) {
    sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

Q_DECL_EXPORT void type_info_callbacks_buildTypeInfo(NetTypeInfoContainer* type) {
    NetTypeInfoContainer* typeCopy = new NetTypeInfoContainer{type->netTypeInfo};
    sharedCallbacks.buildTypeInfo(typeCopy);
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
    NetVariantContainer* resultCopy = NULL;

    if(result != NULL) {
        resultCopy = new NetVariantContainer{result->variant};
    }

    sharedCallbacks.readProperty(propertyCopy, targetCopy, resultCopy);
}

Q_DECL_EXPORT void type_info_callbacks_writeProperty(NetPropertyInfoContainer* property, NetReferenceContainer* target, NetVariantContainer* value) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetPropertyInfoContainer* propertyCopy = new NetPropertyInfoContainer{property->property};
    NetReferenceContainer* targetCopy = new NetReferenceContainer{target->instance};
    NetVariantContainer* valueCopy = NULL;

    if(value != NULL) {
        valueCopy = new NetVariantContainer{value->variant};
    }

    sharedCallbacks.writeProperty(propertyCopy, targetCopy, valueCopy);
}

Q_DECL_EXPORT void type_info_callbacks_invokeMethod(NetMethodInfoContainer* method, NetReferenceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.

    NetMethodInfoContainer* methodCopy = new NetMethodInfoContainer{method->method};
    NetReferenceContainer* targetCopy = new NetReferenceContainer{target->instance};
    NetVariantListContainer* parametersCopy = NULL;
    NetVariantContainer* resultCopy = NULL;

    if(parameters != NULL) {
        parametersCopy = new NetVariantListContainer{parameters->list};
    }
    if(result != NULL) {
        resultCopy = new NetVariantContainer{result->variant};
    }

    sharedCallbacks.invokeMethod(
                methodCopy,
                targetCopy,
                parametersCopy,
                resultCopy);
}

}

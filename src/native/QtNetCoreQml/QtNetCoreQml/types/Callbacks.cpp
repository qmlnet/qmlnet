#include <QtNetCoreQml/types/Callbacks.h>

typedef bool (*isTypeValidCb)(LPWSTR typeName);
typedef void (*buildTypeInfoCb)(NetTypeInfoContainer* typeInfo);
typedef void (*releaseGCHandleCb)(NetGCHandle* handle);
typedef NetGCHandle* (*instantiateTypeCb)(LPWSTR typeName);
typedef void (*readPropertyCb)(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* result);
typedef void (*writePropertyCb)(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* value);
typedef void (*invokeMethodCb)(NetMethodInfoContainer* method, NetInstanceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result);

struct Q_DECL_EXPORT NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    buildTypeInfoCb buildTypeInfo;
    releaseGCHandleCb releaseGCHandle;
    instantiateTypeCb instantiateType;
    readPropertyCb readProperty;
    writePropertyCb writeProperty;
    invokeMethodCb invokeMethod;
};

static NetTypeInfoManagerCallbacks sharedCallbacks;

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

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

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetInstance> target, QSharedPointer<NetVariant> result) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetInstanceContainer* targetContainer = new NetInstanceContainer();
    targetContainer->instance = target;
    NetVariantContainer* valueContainer = new NetVariantContainer();
    valueContainer->variant = result;
    sharedCallbacks.readProperty(propertyContainer, targetContainer, valueContainer);
    // The callbacks dispose of the types.
}

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetInstance> target, QSharedPointer<NetVariant> value) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = property;
    NetInstanceContainer* targetContainer = new NetInstanceContainer();
    targetContainer->instance = target;
    NetVariantContainer* resultContainer = new NetVariantContainer();
    resultContainer->variant = value;
    sharedCallbacks.writeProperty(propertyContainer, targetContainer, resultContainer);
    // The callbacks dispose of the types.
}

void invokeNetMethod(QSharedPointer<NetMethodInfo> method, QSharedPointer<NetInstance> target, QSharedPointer<NetVariantList> parameters, QSharedPointer<NetVariant> result) {
    NetMethodInfoContainer* methodContainer = new NetMethodInfoContainer();
    methodContainer->method = method;
    NetInstanceContainer* targetContainer = new NetInstanceContainer();
    targetContainer->instance = target;
    NetVariantListContainer* parametersContainer = new NetVariantListContainer();
    parametersContainer->list = parameters;
    NetVariantContainer* resultContainer = NULL;
    if(result != NULL) {
        // There is a return type.
        resultContainer = new NetVariantContainer();
        resultContainer->variant = result;
    }
    sharedCallbacks.invokeMethod(methodContainer, targetContainer, parametersContainer, resultContainer);
    // The callbacks dispose of types.
}

extern "C" {

Q_DECL_EXPORT void type_info_callbacks_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

Q_DECL_EXPORT bool type_info_callbacks_isTypeValid(LPWSTR typeName) {
    return sharedCallbacks.isTypeValid(typeName);
}

Q_DECL_EXPORT void type_info_callbacks_releaseGCHandle(NetGCHandle* handle) {
    sharedCallbacks.releaseGCHandle(handle);
}

Q_DECL_EXPORT void type_info_callbacks_buildTypeInfo(NetTypeInfoContainer* type) {
    NetTypeInfoContainer* typeCopy = new NetTypeInfoContainer{type->netTypeInfo};
    sharedCallbacks.buildTypeInfo(typeCopy);
}

Q_DECL_EXPORT NetGCHandle* type_info_callbacks_instantiateType(LPWSTR typeName) {
    return sharedCallbacks.instantiateType(typeName);
}

Q_DECL_EXPORT void type_info_callbacks_readProperty(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* result) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetPropertyInfoContainer* propertyCopy = new NetPropertyInfoContainer{property->property};
    NetInstanceContainer* targetCopy = new NetInstanceContainer{target->instance};
    NetVariantContainer* resultCopy = NULL;

    if(result != NULL) {
        resultCopy = new NetVariantContainer{result->variant};
    }

    sharedCallbacks.readProperty(propertyCopy, targetCopy, resultCopy);
}

Q_DECL_EXPORT void type_info_callbacks_writeProperty(NetPropertyInfoContainer* property, NetInstanceContainer* target, NetVariantContainer* value) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.
    NetPropertyInfoContainer* propertyCopy = new NetPropertyInfoContainer{property->property};
    NetInstanceContainer* targetCopy = new NetInstanceContainer{target->instance};
    NetVariantContainer* valueCopy = NULL;

    if(value != NULL) {
        valueCopy = new NetVariantContainer{value->variant};
    }

    sharedCallbacks.writeProperty(propertyCopy, targetCopy, valueCopy);
}

Q_DECL_EXPORT void type_info_callbacks_invokeMethod(NetMethodInfoContainer* method, NetInstanceContainer* target, NetVariantListContainer* parameters, NetVariantContainer* result) {
    // The parameters have to be copied to new containers, because the callback
    // will delete them.

    NetMethodInfoContainer* methodCopy = new NetMethodInfoContainer{method->method};
    NetInstanceContainer* targetCopy = new NetInstanceContainer{target->instance};
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

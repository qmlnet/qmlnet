#include <QmlNet/types/Callbacks.h>
#include <utility>

namespace QmlNet {

using isTypeValidCb = uchar (*)(LPWSTR);
using createLazyTypeInfoCb = void (*)(NetTypeInfoContainer *);
using loadTypeInfoCb = void (*)(NetTypeInfoContainer *);
using callComponentCompletedCb = void (*)(NetReferenceContainer *);
using callObjectDestroyedCb = void (*)(NetReferenceContainer *);
using releaseNetReferenceCb = void (*)(uint64_t);
using releaseNetDelegateGCHandleCb = void (*)(void *);
using instantiateTypeCb = NetReferenceContainer *(*)(NetTypeInfoContainer *);
using readPropertyCb = void (*)(NetPropertyInfoContainer *, NetReferenceContainer *, NetVariantContainer *, NetVariantContainer *);
using writePropertyCb = void (*)(NetPropertyInfoContainer *, NetReferenceContainer *, NetVariantContainer *, NetVariantContainer *);
using invokeMethodCb = void (*)(NetMethodInfoContainer *, NetReferenceContainer *, NetVariantListContainer *, NetVariantContainer *);
using gcCollectCb = void (*)(int);
using raiseNetSignalsCb = uchar (*)(NetReferenceContainer *, LPWCSTR, NetVariantListContainer *);
using awaitTaskCb = void (*)(NetReferenceContainer *, NetJSValueContainer *, NetJSValueContainer *);
using serializeNetToStringCb = uchar (*)(NetReferenceContainer *, NetVariantContainer *);
using invokeDelegateCb = void (*)(NetReferenceContainer *, NetVariantListContainer *);

struct Q_DECL_EXPORT NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
    createLazyTypeInfoCb createLazyTypeInfo;
    loadTypeInfoCb loadTypeInfo;
    callComponentCompletedCb callComponentCompleted;
    callObjectDestroyedCb callObjectDestroyed;
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
    invokeDelegateCb invokeDelegate;
};

static NetTypeInfoManagerCallbacks sharedCallbacks;

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

bool isTypeValid(const QString& type) {
    static_assert (std::is_pointer<LPWSTR>::value, "Check the cast below.");
    static_assert (!std::is_pointer<std::remove_pointer<LPWSTR>::type>::value, "Check the cast below.");
    static_assert (sizeof(std::remove_pointer<LPWSTR>::type) == sizeof(ushort), "Check the cast below.");
    return sharedCallbacks.isTypeValid(static_cast<LPWSTR>(const_cast<void*>(static_cast<const void*>(type.utf16())))) == 1;
}

void releaseNetReference(uint64_t objectId) {
    return sharedCallbacks.releaseNetReference(objectId);
}

void releaseNetDelegateGCHandle(NetGCHandle* handle) {
    return sharedCallbacks.releaseNetDelegateGCHandle(handle);
}

void createLazyTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = std::move(typeInfo);
    sharedCallbacks.createLazyTypeInfo(container);
}

void loadTypeInfo(QSharedPointer<NetTypeInfo> typeInfo) {
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = std::move(typeInfo);
    sharedCallbacks.loadTypeInfo(container);
}

QSharedPointer<NetReference> instantiateType(QSharedPointer<NetTypeInfo> type) {
    NetTypeInfoContainer* typeContainer = new NetTypeInfoContainer{ std::move(type) }; // .NET will delete this type
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

void callComponentCompleted(QSharedPointer<NetReference> target)
{
    sharedCallbacks.callComponentCompleted(new NetReferenceContainer{target});
}

void callObjectDestroyed(QSharedPointer<NetReference> target)
{
    sharedCallbacks.callObjectDestroyed(new NetReferenceContainer{target});
}

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, const QSharedPointer<NetVariant>& indexParameter, QSharedPointer<NetVariant> result) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = std::move(property);
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = std::move(target);
    NetVariantContainer* indexParameterContainer = nullptr;
    if(indexParameter != nullptr) {
        indexParameterContainer = new NetVariantContainer{indexParameter};
    }
    NetVariantContainer* valueContainer = new NetVariantContainer();
    valueContainer->variant = std::move(result);
    sharedCallbacks.readProperty(propertyContainer, targetContainer, indexParameterContainer, valueContainer);
    // The callbacks dispose of the types.
}

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, const QSharedPointer<NetVariant>& indexParameter, QSharedPointer<NetVariant> value) {
    NetPropertyInfoContainer* propertyContainer = new NetPropertyInfoContainer();
    propertyContainer->property = std::move(property);
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = std::move(target);
    NetVariantContainer* indexParameterContainer = nullptr;
    if(indexParameter != nullptr) {
        indexParameterContainer = new NetVariantContainer{indexParameter};
    }
    NetVariantContainer* resultContainer = new NetVariantContainer();
    resultContainer->variant = std::move(value);
    sharedCallbacks.writeProperty(propertyContainer, targetContainer, indexParameterContainer, resultContainer);
    // The callbacks dispose of the types.
}

void invokeNetMethod(QSharedPointer<NetMethodInfo> method, QSharedPointer<NetReference> target, QSharedPointer<NetVariantList> parameters, const QSharedPointer<NetVariant>& result) {
    NetMethodInfoContainer* methodContainer = new NetMethodInfoContainer();
    methodContainer->method = std::move(method);
    NetReferenceContainer* targetContainer = new NetReferenceContainer();
    targetContainer->instance = std::move(target);
    NetVariantListContainer* parametersContainer = new NetVariantListContainer();
    parametersContainer->list = std::move(parameters);
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

bool raiseNetSignals(QSharedPointer<NetReference> target, const QString& signalName, const QSharedPointer<NetVariantList>& parameters) {
    NetReferenceContainer* targetContainer = new NetReferenceContainer{std::move(target)};
    NetVariantListContainer* parametersContainer = nullptr;
    if(parameters != nullptr) {
        parametersContainer = new NetVariantListContainer{parameters};
    }
    static_assert (std::is_pointer<LPWCSTR>::value, "Check the cast below.");
    static_assert (!std::is_pointer<std::remove_pointer<LPWCSTR>::type>::value, "Check the cast below.");
    static_assert (sizeof(std::remove_pointer<LPWCSTR>::type) == sizeof(ushort), "Check the cast below.");
    return sharedCallbacks.raiseNetSignals(targetContainer, static_cast<LPWCSTR>(static_cast<const void*>(signalName.utf16())), parametersContainer) == 1;
}

void awaitTask(QSharedPointer<NetReference> target, QSharedPointer<NetJSValue> successCallback, const QSharedPointer<NetJSValue>& failureCallback) {
    NetReferenceContainer* targetContainer = new NetReferenceContainer{std::move(target)};
    NetJSValueContainer* sucessCallbackContainer = new NetJSValueContainer{std::move(successCallback)};
    NetJSValueContainer* failureCallbackContainer = nullptr;
    if(failureCallback != nullptr) {
        failureCallbackContainer = new NetJSValueContainer{failureCallback};
    }
    sharedCallbacks.awaitTask(targetContainer, sucessCallbackContainer, failureCallbackContainer);
}

bool serializeNetToString(QSharedPointer<NetReference> instance, QSharedPointer<NetVariant> result)
{
    return sharedCallbacks.serializeNetToString(new NetReferenceContainer{std::move(instance)},
                                                new NetVariantContainer{std::move(result)}) == 1;
}

void invokeDelegate(QSharedPointer<NetReference> del, QSharedPointer<NetVariantList> parameters)
{
    sharedCallbacks.invokeDelegate(new NetReferenceContainer{std::move(del)},
                                   new NetVariantListContainer{std::move(parameters)});
}

}

extern "C" {

Q_DECL_EXPORT void type_info_callbacks_registerCallbacks(QmlNet::NetTypeInfoManagerCallbacks* callbacks) {
    QmlNet::sharedCallbacks = *callbacks;
}

Q_DECL_EXPORT uchar type_info_callbacks_isTypeValid(LPWSTR typeName) {
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

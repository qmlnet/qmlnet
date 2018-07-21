#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetSignalInfo.h>

NetTypeInfo::NetTypeInfo(QString fullTypeName) :
    metaObject(NULL),
    _fullTypeName(fullTypeName),
    _variantType(NetVariantTypeEnum_Invalid) {

}


NetTypeInfo::~NetTypeInfo() {

}

QString NetTypeInfo::getFullTypeName() {
    return _fullTypeName;
}

QString NetTypeInfo::getClassName() {
    return _className;
}

void NetTypeInfo::setClassName(QString className) {
    _className = className;
}

NetVariantTypeEnum NetTypeInfo::getPrefVariantType() {
    return _variantType;
}

void NetTypeInfo::setPrefVariantType(NetVariantTypeEnum variantType) {
    _variantType = variantType;
}

void NetTypeInfo::addMethod(QSharedPointer<NetMethodInfo> methodInfo) {
    _methods.append(methodInfo);
}

uint NetTypeInfo::getMethodCount() {
    return _methods.size();
}

QSharedPointer<NetMethodInfo> NetTypeInfo::getMethodInfo(uint index) {
    if(index >= (uint)_methods.length()) return QSharedPointer<NetMethodInfo>(NULL);
    return _methods.at(index);
}

void NetTypeInfo::addProperty(QSharedPointer<NetPropertyInfo> property) {
    _properties.append(property);
}

uint NetTypeInfo::getPropertyCount() {
    return _properties.size();
}

QSharedPointer<NetPropertyInfo> NetTypeInfo::getProperty(uint index) {
    if(index >= (uint)_properties.length()) return QSharedPointer<NetPropertyInfo>(NULL);
    return _properties.at(index);
}

void NetTypeInfo::addSignal(QSharedPointer<NetSignalInfo> signal) {
    _signals.append(signal);
}

uint NetTypeInfo::getSignalCount() {
    return _signals.size();
}

QSharedPointer<NetSignalInfo> NetTypeInfo::getSignal(uint index) {
    if(index >= (uint)_signals.size()) return QSharedPointer<NetSignalInfo>(NULL);
    return _signals.at(index);
}

extern "C" {

Q_DECL_EXPORT NetTypeInfoContainer* type_info_create(LPWSTR fullTypeName) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = QSharedPointer<NetTypeInfo>(new NetTypeInfo(QString::fromUtf16((const char16_t*)fullTypeName)));
    return result;
}

Q_DECL_EXPORT void type_info_destroy(NetTypeInfoContainer* netTypeInfo) {
    delete netTypeInfo;
    netTypeInfo = NULL;
}

Q_DECL_EXPORT LPWSTR type_info_getFullTypeName(NetTypeInfoContainer* netTypeInfo) {
    return (LPWSTR)netTypeInfo->netTypeInfo->getFullTypeName().utf16();
}

Q_DECL_EXPORT LPWSTR type_info_getClassName(NetTypeInfoContainer* netTypeInfo) {
    return (LPWSTR)netTypeInfo->netTypeInfo->getClassName().utf16();
}

Q_DECL_EXPORT void type_info_setClassName(NetTypeInfoContainer* netTypeInfo, LPWSTR className) {
    netTypeInfo->netTypeInfo->setClassName(QString::fromUtf16((const char16_t*)className));
}

Q_DECL_EXPORT NetVariantTypeEnum type_info_getPrefVariantType(NetTypeInfoContainer* netTypeInfo) {
    return netTypeInfo->netTypeInfo->getPrefVariantType();
}

Q_DECL_EXPORT void type_info_setPrefVariantType(NetTypeInfoContainer* netTypeInfo, NetVariantTypeEnum variantType) {
    netTypeInfo->netTypeInfo->setPrefVariantType(variantType);
}

Q_DECL_EXPORT void type_info_addMethod(NetTypeInfoContainer* netTypeInfo, NetMethodInfoContainer* methodInfo) {
    netTypeInfo->netTypeInfo->addMethod(methodInfo->method);
}

Q_DECL_EXPORT uint type_info_getMethodCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getMethodCount();
}

Q_DECL_EXPORT NetMethodInfoContainer* type_info_getMethodInfo(NetTypeInfoContainer* container, uint index) {
    QSharedPointer<NetMethodInfo> methodInfo = container->netTypeInfo->getMethodInfo(index);
    if(methodInfo == NULL) {
        return NULL;
    }
    NetMethodInfoContainer* result = new NetMethodInfoContainer();
    result->method = methodInfo;
    return result;
}

Q_DECL_EXPORT void type_info_addProperty(NetTypeInfoContainer* container, NetPropertyInfoContainer* propertyContainer) {
    container->netTypeInfo->addProperty(propertyContainer->property);
}

Q_DECL_EXPORT uint type_info_getPropertyCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getPropertyCount();
}

Q_DECL_EXPORT NetPropertyInfoContainer* type_info_getProperty(NetTypeInfoContainer* container, uint index) {
    QSharedPointer<NetPropertyInfo> property = container->netTypeInfo->getProperty(index);
    if(property == NULL) {
        return NULL;
    }
    NetPropertyInfoContainer* result = new NetPropertyInfoContainer();
    result->property = property;
    return result;
}

Q_DECL_EXPORT void type_info_addSignal(NetTypeInfoContainer* container, NetSignalInfoContainer* signalContainer) {
    container->netTypeInfo->addSignal(signalContainer->signal);
}

Q_DECL_EXPORT uint type_info_getSignalCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getSignalCount();
}

Q_DECL_EXPORT NetSignalInfoContainer* type_info_getSignal(NetTypeInfoContainer* container, uint index) {
    QSharedPointer<NetSignalInfo> signal = container->netTypeInfo->getSignal(index);
    if(signal == NULL) {
        return NULL;
    }
    NetSignalInfoContainer* result = new NetSignalInfoContainer();
    result->signal = signal;
    return result;
}

}

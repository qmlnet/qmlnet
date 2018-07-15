#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/types/NetMethodInfo.h>

NetTypeInfo::NetTypeInfo(QString fullTypeName) :
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

extern "C" {

NetTypeInfoContainer* type_info_create(LPWSTR fullTypeName) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = QSharedPointer<NetTypeInfo>(new NetTypeInfo(QString::fromUtf16(fullTypeName)));
    return result;
}

void type_info_destroy(NetTypeInfoContainer* netTypeInfo) {
    delete netTypeInfo;
    netTypeInfo = NULL;
}

LPWSTR type_info_getFullTypeName(NetTypeInfoContainer* netTypeInfo) {
    return (LPWSTR)netTypeInfo->netTypeInfo->getFullTypeName().utf16();
}

LPWSTR type_info_getClassName(NetTypeInfoContainer* netTypeInfo) {
    return (LPWSTR)netTypeInfo->netTypeInfo->getClassName().utf16();
}

void type_info_setClassName(NetTypeInfoContainer* netTypeInfo, LPWSTR className) {
    netTypeInfo->netTypeInfo->setClassName(QString::fromUtf16(className));
}

NetVariantTypeEnum type_info_getPrefVariantType(NetTypeInfoContainer* netTypeInfo) {
    return netTypeInfo->netTypeInfo->getPrefVariantType();
}

void type_info_setPrefVariantType(NetTypeInfoContainer* netTypeInfo, NetVariantTypeEnum variantType) {
    netTypeInfo->netTypeInfo->setPrefVariantType(variantType);
}

void type_info_addMethod(NetTypeInfoContainer* netTypeInfo, NetMethodInfoContainer* methodInfo) {
    netTypeInfo->netTypeInfo->addMethod(methodInfo->methodInfo);
}

uint type_info_getMethodCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getMethodCount();
}

NetMethodInfoContainer* type_info_getMethodInfo(NetTypeInfoContainer* container, uint index) {
    QSharedPointer<NetMethodInfo> methodInfo = container->netTypeInfo->getMethodInfo(index);
    if(methodInfo == NULL) {
        return NULL;
    }
    NetMethodInfoContainer* result = new NetMethodInfoContainer();
    result->methodInfo = methodInfo;
    return result;
}

}

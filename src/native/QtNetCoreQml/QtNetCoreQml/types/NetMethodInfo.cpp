#include <QtNetCoreQml/types/NetMethodInfo.h>
#include <iostream>

NetMethodInfoArguement::NetMethodInfoArguement(QString name,
                                               QSharedPointer<NetTypeInfo> type) :
    _name(name),
    _type(type) {
}

QString NetMethodInfoArguement::getName() {
    return _name;
}

QSharedPointer<NetTypeInfo> NetMethodInfoArguement::getType() {
    return _type;
}

NetMethodInfo::NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                             QString methodName,
                             QSharedPointer<NetTypeInfo> returnType) :
    _parentTypeInfo(parentTypeInfo),
    _methodName(methodName),
    _returnType(returnType) {
}

QString NetMethodInfo::getMethodName() {
    return _methodName;
}

QSharedPointer<NetTypeInfo> NetMethodInfo::getReturnType() {
    return _returnType;
}

void NetMethodInfo::addParameter(QString name, QSharedPointer<NetTypeInfo> typeInfo) {
    _parameters.append(QSharedPointer<NetMethodInfoArguement>(new NetMethodInfoArguement(name, typeInfo)));
}

uint NetMethodInfo::getParameterCount() {
    return _parameters.size();
}

QSharedPointer<NetMethodInfoArguement> NetMethodInfo::getParameter(uint index) {
    if(index >= (uint)_parameters.length()) return QSharedPointer<NetMethodInfoArguement>(NULL);
    return _parameters.at(index);
}

extern "C" {

void method_info_parameter_destroy(NetMethodInfoArguementContainer* container) {
    delete container;
}

LPWSTR method_info_parameter_getName(NetMethodInfoArguementContainer* container) {
    return (LPWSTR)container->methodArguement->getName().utf16();
}

NetTypeInfoContainer* method_info_parameter_getType(NetMethodInfoArguementContainer* container) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->methodArguement->getType();
    return result;
}

NetMethodInfoContainer* method_info_create(NetTypeInfoContainer* parentTypeContainer, LPWSTR methodName, NetTypeInfoContainer* returnTypeContainer) {
    NetMethodInfoContainer* result = new NetMethodInfoContainer();

    QSharedPointer<NetTypeInfo> parentType;
    if(parentTypeContainer != NULL) {
        parentType = parentTypeContainer->netTypeInfo;
    }

    QSharedPointer<NetTypeInfo> returnType;
    if(returnTypeContainer != NULL) {
        returnType = returnTypeContainer->netTypeInfo;
    }

    NetMethodInfo* instance = new NetMethodInfo(parentType, QString::fromUtf16(methodName), returnType);
    result->method = QSharedPointer<NetMethodInfo>(instance);
    return result;
}

void method_info_destroy(NetMethodInfoContainer* container) {
    delete container;
}

LPWSTR method_info_getMethodName(NetMethodInfoContainer* container) {
    return (LPWSTR)container->method->getMethodName().utf16();
}

NetTypeInfoContainer* method_info_getReturnType(NetMethodInfoContainer* container) {
    QSharedPointer<NetTypeInfo> returnType = container->method->getReturnType();
    if(returnType == NULL) {
        return NULL;
    }
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = returnType;
    return result;
}

void method_info_addParameter(NetMethodInfoContainer* container, LPWSTR name, NetTypeInfoContainer* typeInfoContainer) {
    container->method->addParameter(QString::fromUtf16(name), typeInfoContainer->netTypeInfo);
}

uint method_info_getParameterCount(NetMethodInfoContainer* container) {
    return container->method->getParameterCount();
}

NetMethodInfoArguementContainer* method_info_getParameter(NetMethodInfoContainer* container, uint index) {
    QSharedPointer<NetMethodInfoArguement> parameter = container->method->getParameter(index);
    if(parameter == NULL) {
        return NULL;
    }
    NetMethodInfoArguementContainer* result = new NetMethodInfoArguementContainer();
    result->methodArguement = parameter;
    return result;
}

}

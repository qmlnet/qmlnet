#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <iostream>

NetMethodInfoArguement::NetMethodInfoArguement(QString name,
                                               QSharedPointer<NetTypeInfo> type) :
    _name(name),
    _type(type)
{
}

QString NetMethodInfoArguement::getName()
{
    return _name;
}

QSharedPointer<NetTypeInfo> NetMethodInfoArguement::getType()
{
    return _type;
}

NetMethodInfo::NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                             QString methodName,
                             QSharedPointer<NetTypeInfo> returnType) :
    _parentTypeInfo(parentTypeInfo),
    _methodName(methodName),
    _returnType(returnType)
{
}

QString NetMethodInfo::getMethodName()
{
    return _methodName;
}

QSharedPointer<NetTypeInfo> NetMethodInfo::getReturnType()
{
    return _returnType;
}

void NetMethodInfo::addParameter(QString name, QSharedPointer<NetTypeInfo> typeInfo)
{
    _parameters.append(QSharedPointer<NetMethodInfoArguement>(new NetMethodInfoArguement(name, typeInfo)));
}

int NetMethodInfo::getParameterCount()
{
    return _parameters.size();
}

QSharedPointer<NetMethodInfoArguement> NetMethodInfo::getParameter(int index)
{
    if(index < 0) return QSharedPointer<NetMethodInfoArguement>(nullptr);
    if(index >= _parameters.length()) return QSharedPointer<NetMethodInfoArguement>(nullptr);
    return _parameters.at(index);
}

QString NetMethodInfo::getSignature()
{
    QString signature = _methodName;

    if(signature.at(0).isUpper()) {
        signature.replace(0,1, signature.at(0).toLower());
    }

    signature.append("(");

    for(int parameterIndex = 0; parameterIndex <= _parameters.size() - 1; parameterIndex++)
    {
        QSharedPointer<NetMethodInfoArguement> parameter = _parameters.at(parameterIndex);
        QSharedPointer<NetTypeInfo> parameterType = parameter->getType();
        if(parameterIndex > 0) {
            signature.append(",");
        }
        signature.append(NetMetaValueQmlType(parameterType->getPrefVariantType()));
    }

    signature.append(")");

    return signature;
}

extern "C" {

Q_DECL_EXPORT void method_info_parameter_destroy(NetMethodInfoArguementContainer* container)
{
    delete container;
}

Q_DECL_EXPORT LPWSTR method_info_parameter_getName(NetMethodInfoArguementContainer* container)
{
    return (LPWSTR)container->methodArguement->getName().utf16();
}

Q_DECL_EXPORT NetTypeInfoContainer* method_info_parameter_getType(NetMethodInfoArguementContainer* container)
{
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->methodArguement->getType();
    return result;
}

Q_DECL_EXPORT NetMethodInfoContainer* method_info_create(NetTypeInfoContainer* parentTypeContainer, LPWSTR methodName, NetTypeInfoContainer* returnTypeContainer)
{
    NetMethodInfoContainer* result = new NetMethodInfoContainer();

    QSharedPointer<NetTypeInfo> parentType;
    if(parentTypeContainer != nullptr) {
        parentType = parentTypeContainer->netTypeInfo;
    }

    QSharedPointer<NetTypeInfo> returnType;
    if(returnTypeContainer != nullptr) {
        returnType = returnTypeContainer->netTypeInfo;
    }

    NetMethodInfo* instance = new NetMethodInfo(parentType, QString::fromUtf16((const char16_t*)methodName), returnType);
    result->method = QSharedPointer<NetMethodInfo>(instance);
    return result;
}

Q_DECL_EXPORT void method_info_destroy(NetMethodInfoContainer* container)
{
    delete container;
}

Q_DECL_EXPORT LPWSTR method_info_getMethodName(NetMethodInfoContainer* container)
{
    return (LPWSTR)container->method->getMethodName().utf16();
}

Q_DECL_EXPORT NetTypeInfoContainer* method_info_getReturnType(NetMethodInfoContainer* container)
{
    QSharedPointer<NetTypeInfo> returnType = container->method->getReturnType();
    if(returnType == nullptr) {
        return nullptr;
    }
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = returnType;
    return result;
}

Q_DECL_EXPORT void method_info_addParameter(NetMethodInfoContainer* container, LPWSTR name, NetTypeInfoContainer* typeInfoContainer)
{
    container->method->addParameter(QString::fromUtf16((const char16_t*)name), typeInfoContainer->netTypeInfo);
}

Q_DECL_EXPORT int method_info_getParameterCount(NetMethodInfoContainer* container)
{
    return container->method->getParameterCount();
}

Q_DECL_EXPORT NetMethodInfoArguementContainer* method_info_getParameter(NetMethodInfoContainer* container, int index)
{
    QSharedPointer<NetMethodInfoArguement> parameter = container->method->getParameter(index);
    if(parameter == nullptr) {
        return nullptr;
    }
    NetMethodInfoArguementContainer* result = new NetMethodInfoArguementContainer();
    result->methodArguement = parameter;
    return result;
}

}

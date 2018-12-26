#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <QmlNetUtilities.h>
#include <utility>

NetMethodInfoArguement::NetMethodInfoArguement(QString name,
                                               QSharedPointer<NetTypeInfo> type) :
    _name(std::move(name)),
    _type(std::move(type))
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
                             QSharedPointer<NetTypeInfo> returnType,
                             bool isStatic) :
    _parentTypeInfo(std::move(parentTypeInfo)),
    _methodName(std::move(methodName)),
    _returnType(std::move(returnType)),
    _isStatic(isStatic)
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

bool NetMethodInfo::isStatic()
{
    return _isStatic;
}

void NetMethodInfo::addParameter(QString name, QSharedPointer<NetTypeInfo> typeInfo)
{
    _parameters.append(QSharedPointer<NetMethodInfoArguement>(new NetMethodInfoArguement(std::move(name), std::move(typeInfo))));
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

Q_DECL_EXPORT QmlNetStringContainer* method_info_parameter_getName(NetMethodInfoArguementContainer* container)
{
    QString result = container->methodArguement->getName();
    return createString(result);
}

Q_DECL_EXPORT NetTypeInfoContainer* method_info_parameter_getType(NetMethodInfoArguementContainer* container)
{
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->methodArguement->getType();
    return result;
}

Q_DECL_EXPORT NetMethodInfoContainer* method_info_create(NetTypeInfoContainer* parentTypeContainer, LPWSTR methodName, NetTypeInfoContainer* returnTypeContainer, uchar isStatic)
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

    NetMethodInfo* instance = new NetMethodInfo(parentType, QString::fromUtf16(static_cast<const char16_t*>(methodName)), returnType, isStatic == 1 ? true : false);
    result->method = QSharedPointer<NetMethodInfo>(instance);
    return result;
}

Q_DECL_EXPORT void method_info_destroy(NetMethodInfoContainer* container)
{
    delete container;
}

Q_DECL_EXPORT QmlNetStringContainer* method_info_getMethodName(NetMethodInfoContainer* container)
{
    QString methodName = container->method->getMethodName();
    return createString(methodName);
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

Q_DECL_EXPORT uchar method_info_isStatic(NetMethodInfoContainer* container)
{
    if(container->method->isStatic()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT void method_info_addParameter(NetMethodInfoContainer* container, LPWSTR name, NetTypeInfoContainer* typeInfoContainer)
{
    container->method->addParameter(QString::fromUtf16(static_cast<const char16_t*>(name)), typeInfoContainer->netTypeInfo);
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

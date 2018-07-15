#include <QtNetCoreQml/types/NetMethodInfo.h>

NetMethodInfo::NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                             QString methodName,
                             QSharedPointer<NetTypeInfo> returnType) :
    _parentTypeInfo(parentTypeInfo),
    _methodName(methodName),
    _returnType(returnType)
{

}

QString NetMethodInfo::GetMethodName()
{
    return _methodName;
}

QSharedPointer<NetTypeInfo> NetMethodInfo::GetReturnType()
{
    return _returnType;
}

extern "C" {

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
    result->methodInfo = QSharedPointer<NetMethodInfo>(instance);
    return result;
}

void method_info_destroy(NetMethodInfoContainer* container) {
    delete container;
}

}

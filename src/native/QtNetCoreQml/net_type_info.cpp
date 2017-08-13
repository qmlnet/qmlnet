#include "net_type_info.h"
#include <QDebug>

NetTypeInfoCallbacks* NetTypeInfoManager::callbacks = NULL;
QMap<QString, NetTypeInfo*> NetTypeInfoManager::types;

NetTypeInfo::NetTypeInfo(std::string typeName) :
    metaObject(NULL)
{
    this->typeName = typeName;
}

NetTypeInfo::~NetTypeInfo()
{

}

NetMethodInfo::NetMethodInfo(NetTypeInfo* parentTypeInfo, std::string methodName) :
    returnType(NULL),
    parentTypeInfo(parentTypeInfo),
    methodName(methodName)
{

}

std::string NetMethodInfo::GetMethodName()
{
    return methodName;
}

void NetMethodInfo::SetReturnType(NetTypeInfo *typeInfo)
{
    returnType = typeInfo;
}

NetTypeInfo* NetMethodInfo::GetReturnType()
{
    return returnType;
}

void NetMethodInfo::AddParameter(std::string parameterName, NetTypeInfo *typeInfo)
{
    parameters.append(NetTypeInfoParameter{ parameterName, typeInfo });
}

int NetMethodInfo::GetParameterCount()
{
    return parameters.length();
}

void NetMethodInfo::GetParameterInfo(int index, std::string *parameterName, NetTypeInfo **typeInfo)
{
    if(index < 0) return;
    if(index >= parameters.length()) return;

    *parameterName = parameters.at(0).name;
    *typeInfo = parameters.at(0).typeInfo;
}

std::string NetTypeInfo::GetTypeName()
{
    return typeName;
}

void NetTypeInfo::AddMethod(NetMethodInfo* methodInfo)
{
    methods.append(methodInfo);
}

int NetTypeInfo::GetMethodCount()
{
    return methods.length();
}

NetMethodInfo* NetTypeInfo::GetMethod(int index)
{
    if(index < 0) return NULL;
    if(index >= methods.length()) return NULL;

    return methods.at(index);
}

NetTypeInfoManager::NetTypeInfoManager()
{
}

void NetTypeInfoManager::setCallbacks(NetTypeInfoCallbacks* callbacks)
{
    NetTypeInfoManager::callbacks = callbacks;
}

bool NetTypeInfoManager::isValidType(char* typeName)
{
    return NetTypeInfoManager::callbacks->isValidType(typeName);
}

NetTypeInfo* NetTypeInfoManager::GetTypeInfo(char* typeName)
{
    QString key(typeName);

    if(NetTypeInfoManager::types.contains(key))
        return NetTypeInfoManager::types.value(key);

    std::string t(key.toLocal8Bit().constData());

    NetTypeInfo* typeInfo = new NetTypeInfo(t);
    NetTypeInfoManager::types.insert(NetTypeInfoManager::types.end(), key, typeInfo);

    NetTypeInfoManager::callbacks->BuildTypeInfo(typeInfo);

    return typeInfo;
}

NetMethodInfo* NetTypeInfoManager::NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName)
{
    return new NetMethodInfo(parentTypeInfo, std::string(methodName));
}

NetInstance* NetTypeInfoManager::CreateInstance(NetTypeInfo* typeInfo)
{
    NetInstance* instance = new NetInstance();
    NetTypeInfoManager::callbacks->CreateInstance((char*)typeInfo->GetTypeName().c_str(), instance);
    return instance;
}

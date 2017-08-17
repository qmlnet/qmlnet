#include "net_type_info_manager.h"
#include "net_type_info.h"
#include "net_type_info_method.h"
#include "net_type_info_property.h"
#include "net_instance.h"

NetTypeInfoCallbacks* NetTypeInfoManager::callbacks = NULL;
QMap<QString, NetTypeInfo*> NetTypeInfoManager::types;

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

NetMethodInfo* NetTypeInfoManager::NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName, NetTypeInfo* returnType)
{
    return new NetMethodInfo(parentTypeInfo, std::string(methodName), returnType);
}

NetPropertyInfo* NetTypeInfoManager::NewPropertyInfo(NetTypeInfo* parentTypeInfo, std::string propertyName, NetTypeInfo* returnType, bool canRead, bool canWrite)
{
    return new NetPropertyInfo(parentTypeInfo, propertyName, returnType, canRead, canWrite);
}

NetInstance* NetTypeInfoManager::CreateInstance(NetTypeInfo* typeInfo)
{
    NetGCHandle* handle = 0;
    NetTypeInfoManager::callbacks->CreateInstance(typeInfo, &handle);
    return new NetInstance(handle, typeInfo);
}

NetInstance* NetTypeInfoManager::ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target)
{
    // TODO:
    //NetInstance* result = new NetInstance(propertyInfo->GetReturnType()->GetInterType());
    //NetTypeInfoManager::callbacks->ReadProperty(propertyInfo, target, result);
    //return result;
    return NULL;
}

void NetTypeInfoManager::WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetInstance* value)
{
    NetTypeInfoManager::callbacks->WriteProperty(propertyInfo, target, value);
}

NetInstance* NetTypeInfoManager::InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetInstance*> parameters)
{
    // TODO:
//    NetTypeInfo* returnType = methodInfo->GetReturnType();
//    NetInstance* result = NULL;
//    if(returnType) {
//        result = new NetInstance(returnType->GetInterType());
//    }
//    NetTypeInfoManager::callbacks->InvokeMethod(methodInfo, target, parameters, result);
//    return result;
    return NULL;
}

void NetTypeInfoManager::ReleaseGCHandle(NetGCHandle* gcHandle)
{
    NetTypeInfoManager::callbacks->ReleaseGCHandle(gcHandle);
}

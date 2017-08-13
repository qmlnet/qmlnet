#include "net_type_info.h"
#include <QDebug>

NetTypeInfoCallbacks* NetTypeInfoManager::callbacks = NULL;
QMap<QString, NetTypeInfo*> NetTypeInfoManager::types;

NetTypeInfo::NetTypeInfo(std::string typeName)
{
    this->typeName = typeName;
}

NetTypeInfo::~NetTypeInfo()
{

}

std::string NetTypeInfo::GetTypeName()
{
    return typeName;
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

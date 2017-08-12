#include "net_type_info.h"
#include <QDebug>

NetTypeInfoCallbacks* NetTypeInfoManager::callbacks = NULL;

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

#include "net_type_info_property.h"

NetPropertyInfo::NetPropertyInfo(NetTypeInfo* parentTypeInfo,
        std::string propertyName,
        NetTypeInfo* returnType,
        bool canRead,
        bool canWrite,
        std::string notifySignalName) :
    parentTypeInfo(parentTypeInfo),
    propertyName(propertyName),
    returnType(returnType),
    canRead(canRead),
    canWrite(canWrite),
    notifySignalName(notifySignalName)
{

}

NetTypeInfo* NetPropertyInfo::GetParentType()
{
    return parentTypeInfo;
}

std::string NetPropertyInfo::GetPropertyName()
{
    return propertyName;
}

NetTypeInfo* NetPropertyInfo::GetReturnType()
{
    return returnType;
}

bool NetPropertyInfo::CanRead()
{
    return canRead;
}

bool NetPropertyInfo::CanWrite()
{
    return canWrite;
}

std::string NetPropertyInfo::GetNotifySignalName()
{
    return notifySignalName;
}

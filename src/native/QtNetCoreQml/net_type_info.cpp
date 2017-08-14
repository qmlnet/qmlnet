#include "net_type_info.h"
#include <QDebug>

NetTypeInfo::NetTypeInfo(std::string typeName, NetInterTypeEnum interType) :
    typeName(typeName),
    interType(interType),
    metaObject(NULL)
{

}

NetTypeInfo::~NetTypeInfo()
{

}





std::string NetTypeInfo::GetTypeName()
{
    return typeName;
}

NetInterTypeEnum NetTypeInfo::GetInterType()
{
    return interType;
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

void NetTypeInfo::AddProperty(NetPropertyInfo* propertyInfo)
{
    properties.append(propertyInfo);
}

int NetTypeInfo::GetPropertyCount()
{
    return properties.length();
}

NetPropertyInfo* NetTypeInfo::GetProperty(int index)
{
    if(index < 0) return NULL;
    if(index >= properties.length()) return NULL;

    return properties.at(index);
}

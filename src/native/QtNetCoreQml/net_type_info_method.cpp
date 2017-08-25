#include "net_type_info_method.h"

NetMethodInfo::NetMethodInfo(NetTypeInfo* parentTypeInfo,
                             std::string methodName,
                             NetTypeInfo *returnType) :
    parentTypeInfo(parentTypeInfo),
    methodName(methodName),
    returnType(returnType)
{

}

std::string NetMethodInfo::GetMethodName()
{
    return methodName;
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

    if(parameterName)
        *parameterName = parameters.at(index).name;
    if(typeInfo)
        *typeInfo = parameters.at(index).typeInfo;
}

#ifndef NET_TYPE_INFO_METHOD_H
#define NET_TYPE_INFO_METHOD_H

#include <QList>

class NetTypeInfo;

struct NetTypeInfoParameter
{
    std::string name;
    NetTypeInfo *typeInfo;
};

class NetMethodInfo {
public:
    NetMethodInfo(NetTypeInfo *parentTypeInfo, std::string methodName, NetTypeInfo *returnType);
    std::string GetMethodName();
    NetTypeInfo* GetReturnType();
    void AddParameter(std::string parameterName, NetTypeInfo *typeInfo);
    int GetParameterCount();
    void GetParameterInfo(int index, std::string *parameterName, NetTypeInfo **typeInfo);
private:
    NetTypeInfo *parentTypeInfo;
    std::string methodName;
    NetTypeInfo *returnType;
    QList<NetTypeInfoParameter> parameters;
};

#endif // NET_TYPE_INFO_METHOD_H

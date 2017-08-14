%module(directors="1") example
%{
#include "net_type_info.h"
%}

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName, NetInterTypeEnum interType);
    ~NetTypeInfo();
    std::string GetTypeName();
    NetInterTypeEnum GetInterType();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    void AddProperty(NetPropertyInfo* propertyInfo);
    int GetPropertyCount();
    NetPropertyInfo* GetProperty(int index);
};
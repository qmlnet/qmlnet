%module(directors="1") example
%{
#include "net_type_info.h"
%}

class NetTypeInfo {
public:
    NetTypeInfo(std::string fullTypeName);
    ~NetTypeInfo();
    NetVariantTypeEnum GetPrefVariantType();
    void SetPrefVariantType(NetVariantTypeEnum value);
    std::string GetFullTypeName();
    void SetClassName(std::string className);
    std::string GetClassName();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    void AddProperty(NetPropertyInfo* propertyInfo);
    int GetPropertyCount();
    NetPropertyInfo* GetProperty(int index);
};
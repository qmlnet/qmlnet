%module(directors="1") example
%{
#include "net_type_info_manager.h"
%}

%feature("director") NetTypeInfoCallbacks;

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
	virtual bool isValidType(char* typeName);
    virtual NetInterTypeEnum GetNetInterType(char* typeName);
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo);
    virtual void CreateInstance(char* typeName, NetInstance* instance);
};

class NetTypeInfoManager
{
public:
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetInterTypeEnum GetNetInterType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName, NetTypeInfo* returnType);
    static NetPropertyInfo* NewPropertyInfo(NetTypeInfo* parentTypeInfo, std::string propertyName, NetTypeInfo* returnType, bool canRead, bool canWrite);
    static NetInstance* CreateInstance(NetTypeInfo* typeInfo);
};
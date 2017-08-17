%module(directors="1") example
%{
#include "net_type_info_manager.h"
%}

%feature("director") NetTypeInfoCallbacks;
%template(NetVariantVector) std::vector< NetVariant* >;

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
	virtual bool isValidType(char* typeName);
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo);
    virtual void CreateInstance(NetTypeInfo* typeInfo, NetGCHandle** instance);
    virtual void ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetVariant* result);
    virtual void WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetVariant* value);
    virtual void InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetVariant*> parameters, NetVariant* result);
    virtual void ReleaseGCHandle(NetGCHandle* gcHandle);
};

class NetTypeInfoManager
{
public:
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetInstance* CreateInstance(NetTypeInfo* typeInfo);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName, NetTypeInfo* returnType);
    static NetPropertyInfo* NewPropertyInfo(NetTypeInfo* parentTypeInfo, std::string propertyName, NetTypeInfo* returnType, bool canRead, bool canWrite);
};
%module(directors="1") example
%{
#include "net_type_info.h"
%}

%feature("director") NetTypeInfoCallbacks;

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName);
    std::string GetTypeName();
};

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
	virtual bool isValidType(char* typeName);
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo);
};

class NetTypeInfoManager
{
public:
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    NetTypeInfo* GetTypeInfo(char* typeName);
};
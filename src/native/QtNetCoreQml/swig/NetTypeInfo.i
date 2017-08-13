%module(directors="1") example
%{
#include "net_type_info.h"
%}

%feature("director") NetTypeInfoCallbacks;

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName);
    std::string GetTypeName();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
};

class NetMethodInfo {
public:
    NetMethodInfo(NetTypeInfo *parentTypeInfo, std::string methodName);
    std::string GetMethodName();
    void SetReturnType(NetTypeInfo *typeInfo);
    NetTypeInfo* GetReturnType();
    void AddParameter(std::string parameterName, NetTypeInfo *typeInfo);
    int GetParameterCount();
    void GetParameterInfo(int index, std::string *parameterName, NetTypeInfo **typeInfo);
};

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
	virtual bool isValidType(char* typeName);
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo);
    virtual void CreateInstance(char* typeName, NetInstance* instance);
};

class NetTypeInfoManager
{
public:
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName);
};
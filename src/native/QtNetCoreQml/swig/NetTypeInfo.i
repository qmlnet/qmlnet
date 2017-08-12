%module(directors="1") example
%{
#include "net_type_info.h"
%}

%feature("director") NetTypeInfoCallbacks;

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
	virtual bool isValidType(char* typeName);
};

class NetTypeInfoManager
{
public:
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
};
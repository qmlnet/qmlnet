%{
#include "net_invoker.h"
%}

%typemap(ctype) GetMethodInfoCb "GetMethodInfoCb*"
%typemap(imtype) GetMethodInfoCb "GetMethodInfoCb*"
%typemap(cstype) GetMethodInfoCb "GetMethodInfoCb*"

typedef NetMethodInfo* (*GetMethodInfoCb)(char*);

class NetInvoker {
public:
    static void setGetMethodInfo(GetMethodInfoCb getMethodInfo);
};

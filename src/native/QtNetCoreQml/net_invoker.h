#ifndef NET_INVOKER_H
#define NET_INVOKER_H

#include "net_type_info.h"

typedef NetMethodInfo* (*GetMethodInfoCb)(char*);

class NetInvoker {
public:
    static GetMethodInfoCb getMethodInfo;
    static void setGetMethodInfo(GetMethodInfoCb getMethodInfo);
    static NetMethodInfo* GetMethodInfo(const char* methodName);
};

#endif // NET_INVOKER_H

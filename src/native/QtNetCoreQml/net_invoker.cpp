#include "net_invoker.h"

GetMethodInfoCb NetInvoker::getMethodInfo = NULL;

void NetInvoker::setGetMethodInfo(GetMethodInfoCb getMethodInfo)
{
    NetInvoker::getMethodInfo = getMethodInfo;
}

NetMethodInfo* NetInvoker::GetMethodInfo(const char* methodName)
{
    return NetInvoker::getMethodInfo(const_cast<char*>(methodName));
}

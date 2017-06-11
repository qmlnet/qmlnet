#ifndef NET_INVOKER_H
#define NET_INVOKER_H

#include "net_type_info.h"

class NetInvokerBase {
public:
    virtual ~NetInvokerBase() {}

    virtual bool IsValidType(std::string type)
    {
        return false;
    }

    virtual const NetMethodInfo* GetMethodInfo(std::string tt)
    {
        return NULL;
    }
};

class NetInvoker {
public:
    static NetInvokerBase *invoker;
    static void set(NetInvokerBase* invoker) { NetInvoker::invoker = invoker; }
    static void reset() { NetInvoker::invoker = 0; }
    static bool IsValidType(std::string type)
    {
        return NetInvoker::invoker->IsValidType(type);
    }
};

#endif // NET_INVOKER_H

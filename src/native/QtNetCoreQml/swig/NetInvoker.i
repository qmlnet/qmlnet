%{
#include "net_invoker.h"
%}

%feature("director") NetInvokerBase;

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
  static void set(NetInvokerBase* invoker) { NetInvoker::invoker = invoker; }
  static void reset() { NetInvoker::invoker = 0; }
  static bool IsValidType(std::string type)
  {
      return NetInvoker::invoker->IsValidType(type);
  }
private:
  static NetInvokerBase *invoker;
};

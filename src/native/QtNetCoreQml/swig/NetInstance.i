%{
#include "net_instance.h"
%}

class NetInstance
{
public:
    NetInstance(NetInterTypeEnum interType);
    NetInterTypeEnum GetInterType();
    void SetValue(void* value);
    void* GetValue();
}; 
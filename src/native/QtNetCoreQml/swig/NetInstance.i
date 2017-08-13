%{
#include "net_instance.h"
%}

class NetInstance
{
public:
    NetInstance();
    void SetValue(void* value);
    void* GetValue();
}; 
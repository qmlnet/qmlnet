%{
#include "net_instance.h"
%}

class NetInstance
{
public:
    NetInstance(NetInterTypeEnum interType);
    NetInterTypeEnum GetInterType();
    void SetBool(bool value);
    bool GetBool();
    void SetInt(int value);
    int GetInt();
    void SetValue(void* value);
    void* GetValue();
}; 
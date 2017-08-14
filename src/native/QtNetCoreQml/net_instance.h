#ifndef NET_INSTANCE_H
#define NET_INSTANCE_H

#include "qtnetcoreqml_global.h"

class NetInstance
{
public:
    NetInstance(NetInterTypeEnum interType);
    NetInterTypeEnum GetInterType();
    void SetValue(void* value);
    void* GetValue();
private:
    NetInterTypeEnum interType;
    void* value;
};

#endif // NET_INSTANCE_H

#include "net_instance.h"

NetInstance::NetInstance(NetInterTypeEnum interType) :
    interType(interType),
    value(0)
{

}

NetInterTypeEnum NetInstance::GetInterType()
{
    return interType;
}

void NetInstance::SetValue(void* value)
{
    this->value = value;
}

void* NetInstance::GetValue()
{
    return value;
}

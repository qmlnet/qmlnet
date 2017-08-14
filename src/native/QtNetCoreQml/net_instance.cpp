#include "net_instance.h"

NetInstance::NetInstance(NetInterTypeEnum interType) :
    interType(interType),
    boolValue(false),
    value(0)
{

}

NetInterTypeEnum NetInstance::GetInterType()
{
    return interType;
}

void NetInstance::SetBool(bool value)
{
    boolValue = value;
}

bool NetInstance::GetBool()
{
    return boolValue;
}

void NetInstance::SetValue(void* value)
{
    this->value = value;
}

void* NetInstance::GetValue()
{
    return value;
}

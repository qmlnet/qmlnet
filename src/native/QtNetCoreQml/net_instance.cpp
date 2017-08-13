#include "net_instance.h"

NetInstance::NetInstance() :
    value(0)
{

}

void NetInstance::SetValue(void* value)
{
    this->value = value;
}

void* NetInstance::GetValue()
{
    return value;
}

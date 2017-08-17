#include "net_qml_value.h"
#include "net_type_info_manager.h"

NetValue::NetValue(NetInstance *instance, QObject *parent)
    : instance(instance)
{

    valueMeta = new GoValueMetaObject(this, instance);
    setParent(parent);
}

NetValue::~NetValue()
{
    delete instance;
    instance = NULL;
}

void NetValue::activate(int propIndex)
{
    valueMeta->activatePropIndex(propIndex);
}

#include "net_qml_value.h"
#include "net_type_info_manager.h"
#include "net_instance.h"

NetValue::NetValue(NetInstance *instance, NetTypeInfo *typeInfo, QObject *parent)
    : instance(instance), typeInfo(typeInfo)
{
    valueMeta = new GoValueMetaObject(this, instance, typeInfo);
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

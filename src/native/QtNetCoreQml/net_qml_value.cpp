#include "net_qml_value.h"

NetValue::NetValue(NetInstance *instance, NetTypeInfo *typeInfo, QObject *parent)
    : instance(instance), typeInfo(typeInfo)
{
    valueMeta = new GoValueMetaObject(this, addr, typeInfo);
    setParent(parent);
}

NetValue::~NetValue()
{
    //hookGoValueDestroyed(qmlEngine(this), addr);
}

void NetValue::activate(int propIndex)
{
    valueMeta->activatePropIndex(propIndex);
}

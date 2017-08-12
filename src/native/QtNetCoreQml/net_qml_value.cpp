#include "net_qml_value.h"

NetValue::NetValue(NetAddr *addr, NetTypeInfo *typeInfo, QObject *parent)
    : addr(addr), typeInfo(typeInfo)
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

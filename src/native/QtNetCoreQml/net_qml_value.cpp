#include "net_qml_value.h"

GoValue::GoValue(GoAddr *addr, NetTypeInfo *typeInfo, QObject *parent)
    : addr(addr), typeInfo(typeInfo)
{
    valueMeta = new GoValueMetaObject(this, addr, typeInfo);
    setParent(parent);
}

GoValue::~GoValue()
{
    //hookGoValueDestroyed(qmlEngine(this), addr);
}

void GoValue::activate(int propIndex)
{
    valueMeta->activatePropIndex(propIndex);
}

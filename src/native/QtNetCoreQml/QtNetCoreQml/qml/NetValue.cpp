#include <QtNetCoreQml/qml/NetValue.h>
#include <QtNetCoreQml/qml/NetValueMetaObject.h>

NetValue::NetValue(QSharedPointer<NetInstance> instance, QObject *parent)
    : instance(instance)
{
    valueMeta = new NetValueMetaObject(this, instance);
    setParent(parent);
}

NetValue::~NetValue()
{

}


QSharedPointer<NetInstance> NetValue::getNetInstance()
{
    return instance;
}

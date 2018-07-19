#include <QQmlEngine>
#include <QtNetCoreQml/qml/NetValue.h>
#include <QtNetCoreQml/qml/NetValueMetaObject.h>

NetValue::~NetValue()
{
    auto hit = netValues.find(instance.data());
    if(hit != netValues.end())
    {
        netValues.erase(hit);
    }
    qDebug("NetValue deleted: " + instance->getTypeInfo()->getClassName().toLatin1());
    if(instance != nullptr) {
        instance->release();
    }
}


QSharedPointer<NetInstance> NetValue::getNetInstance()
{
    return instance;
}

NetValue* NetValue::forInstance(QSharedPointer<NetInstance> instance)
{
    if(netValues.find(instance.data()) != netValues.end())
    {
        return netValues.at(instance.data());
    }
    auto result = new NetValue(instance, nullptr);
    QQmlEngine::setObjectOwnership(result, QQmlEngine::JavaScriptOwnership);
    return result;
}

NetValue::NetValue(QSharedPointer<NetInstance> instance, QObject *parent)
    : instance(instance)
{
    valueMeta = new NetValueMetaObject(this, instance);
    setParent(parent);
    netValues[instance.data()] = this;
    qDebug("NetValue created: " + instance->getTypeInfo()->getClassName().toLatin1());
}

std::map<NetInstance*, NetValue*> NetValue::netValues = std::map<NetInstance*, NetValue*>();

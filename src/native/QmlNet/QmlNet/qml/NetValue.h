#ifndef NETVALUE_H
#define NETVALUE_H

#include <map>

#include <QmlNet.h>
#include <QmlNet/types/NetInstance.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QObject>
#include <QSharedPointer>

class NetValueMetaObject;

struct NetValueInterface
{
    virtual QSharedPointer<NetInstance> getNetInstance() = 0;
};

Q_DECLARE_INTERFACE(NetValueInterface, "netcoreqml.NetValueInterface")

class NetValue : public QObject, NetValueInterface
{
    Q_OBJECT
    Q_INTERFACES(NetValueInterface)
public:
    virtual ~NetValue();
    QSharedPointer<NetInstance> getNetInstance();
    bool activateSignal(QString signalName, QSharedPointer<NetVariantList> arguments);

    static NetValue* forInstance(QSharedPointer<NetInstance> instance, bool autoCreate = true);

protected:
    NetValue(QSharedPointer<NetInstance> instance, QObject *parent);

private:
    QSharedPointer<NetInstance> instance;
    NetValueMetaObject* valueMeta;

    static std::map<NetInstance*, NetValue*> netValues;
};

#endif // NETVALUE_H

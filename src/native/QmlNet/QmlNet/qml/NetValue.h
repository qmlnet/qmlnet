#ifndef NETVALUE_H
#define NETVALUE_H

#include <map>

#include <QmlNet.h>
#include <QmlNet/types/NetReference.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QObject>
#include <QSharedPointer>

class NetValueMetaObject;

struct NetValueInterface
{
    virtual QSharedPointer<NetReference> getNetReference() = 0;
};

Q_DECLARE_INTERFACE(NetValueInterface, "netcoreqml.NetValueInterface")

class NetValue : public QObject, NetValueInterface
{
    Q_OBJECT
    Q_INTERFACES(NetValueInterface)
public:
    virtual ~NetValue();
    QSharedPointer<NetReference> getNetReference();
    bool activateSignal(QString signalName, QSharedPointer<NetVariantList> arguments);

    static NetValue* forInstance(QSharedPointer<NetReference> instance, bool autoCreate = true);

protected:
    NetValue(QSharedPointer<NetReference> instance, QObject *parent);

private:
    QSharedPointer<NetReference> instance;
    NetValueMetaObject* valueMeta;

    static std::map<NetReference*, NetValue*> netValues;
};

#endif // NETVALUE_H

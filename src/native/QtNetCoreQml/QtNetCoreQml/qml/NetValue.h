#ifndef NETVALUE_H
#define NETVALUE_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/types/NetInstance.h>
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
    NetValue(QSharedPointer<NetInstance> instance, QObject *parent);
    virtual ~NetValue();
    QSharedPointer<NetInstance> getNetInstance();
private:
    QSharedPointer<NetInstance> instance;
    NetValueMetaObject* valueMeta;
};

#endif // NETVALUE_H

#ifndef NETQOBJECT_H
#define NETQOBJECT_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetVariant;
class NetVariantList;
class NetQObjectSignalConnection;
class NetReference;

class NetQObject
{
public:
    NetQObject(QObject* qObject, bool ownsObject = false);
    ~NetQObject();
    QObject* getQObject();
    QSharedPointer<NetVariant> getProperty(QString propertyName, bool* wasSuccess);
    void setProperty(QString propertyName, QSharedPointer<NetVariant> value, bool* wasSuccess);
    QSharedPointer<NetVariant> invokeMethod(QString methodName, QSharedPointer<NetVariantList> parameters, bool* wasSuccess);
    QSharedPointer<NetQObjectSignalConnection> attachSignal(QString signalName, QSharedPointer<NetReference> delegate, bool* wasSuccess);
    QSharedPointer<NetQObjectSignalConnection> attachNotifySignal(QString propertyName, QSharedPointer<NetReference> delegate, bool* wasSuccess);
private:
    QObject* _qObject;
    bool _ownsObject;
};

struct NetQObjectContainer {
    QSharedPointer<NetQObject> qObject;
};

#endif // NETQOBJECT_H

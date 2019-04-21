#ifndef NETQOBJECTSIGNALCONNECTION_H
#define NETQOBJECTSIGNALCONNECTION_H

#include <QObject>
#include <QSharedPointer>

class NetReference;

class NetQObjectSignalConnectionBase : public QObject
{
    Q_OBJECT
public:
    NetQObjectSignalConnectionBase();
    ~NetQObjectSignalConnectionBase();
    QMetaMethod getSignalHandler();
public slots:
    void signalRaised();
};

class NetQObjectSignalConnection : public NetQObjectSignalConnectionBase
{
public:
    NetQObjectSignalConnection(QSharedPointer<NetReference> delegate, QObject* qObject);
    ~NetQObjectSignalConnection() override;
    int qt_metacall(QMetaObject::Call c, int id, void** a) override;
private:
    QSharedPointer<NetReference> _delegate;
    QObject* _qObject;
};

struct NetQObjectSignalConnectionContainer {
    QSharedPointer<NetQObjectSignalConnection> signalConnection;
};

#endif // NETQOBJECTSIGNALCONNECTION_H

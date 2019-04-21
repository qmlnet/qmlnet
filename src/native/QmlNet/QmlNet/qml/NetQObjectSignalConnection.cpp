#include <QmlNet/qml/NetQObjectSignalConnection.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/types/Callbacks.h>
#include <QMetaMethod>

NetQObjectSignalConnectionBase::NetQObjectSignalConnectionBase()
{

}

NetQObjectSignalConnectionBase::~NetQObjectSignalConnectionBase()
{

}

QMetaMethod NetQObjectSignalConnectionBase::getSignalHandler()
{
    return metaObject()->method(metaObject()->methodOffset());
}

void NetQObjectSignalConnectionBase::signalRaised()
{
    // Dummy, handled in NetQObjectSignalConnection::qt_metacall()
}

NetQObjectSignalConnection::NetQObjectSignalConnection(QSharedPointer<NetReference> delegate,
                                                       QObject* qObject)
    : _delegate(delegate),
      _qObject(qObject)
{

}

NetQObjectSignalConnection::~NetQObjectSignalConnection()
{

}

int NetQObjectSignalConnection::qt_metacall(QMetaObject::Call c, int id, void** a)
{
    if(c == QMetaObject::InvokeMetaMethod) {
        int offset = this->metaObject()->methodOffset();
        if(id < offset) {
            return QObject::qt_metacall(c, id, a);
        }

        if(this->metaObject()->indexOfSlot("signalRaised()") == id) {
            QMetaMethod signal = _qObject->metaObject()->method(senderSignalIndex());

            // Convert signal args to QVariantList
            QSharedPointer<NetVariantList> netParameters = QSharedPointer<NetVariantList>(new NetVariantList());
            for (int i = 0; i < signal.parameterCount(); ++i) {
                QVariant arg = QVariant(signal.parameterType(i), a[i+1]);
                netParameters->add(NetVariant::fromQVariant(&arg));
            }

            QmlNet::invokeDelegate(_delegate, netParameters);

            return -1;
        }
    }

    return id;
}

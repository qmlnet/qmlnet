#ifndef NETVALUEMETAOBJECT_H
#define NETVALUEMETAOBJECT_H

#include <QmlNet.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetValue.h>
#include <private/qobject_p.h>

QMetaObject *metaObjectFor(const QSharedPointer<NetTypeInfo>& typeInfo);

class NetValueMetaObject : public QAbstractDynamicMetaObject
{
public:
    NetValueMetaObject(QObject* value, const QSharedPointer<NetReference>& instance);

protected:
    int metaCall(QMetaObject::Call c, int id, void **a);

private:
    int metaCallRecursive(QMetaObject::Call c, int originalIdx, int idx, void **a, QSharedPointer<NetTypeInfo> typeInfo);

    QObject *value;
    QSharedPointer<NetReference> instance;
};

#endif // NETVALUEMETAOBJECT_H

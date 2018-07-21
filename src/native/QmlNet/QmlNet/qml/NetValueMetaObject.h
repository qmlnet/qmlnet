#ifndef NETVALUEMETAOBJECT_H
#define NETVALUEMETAOBJECT_H

#include <QmlNet.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetValue.h>
#include <private/qobject_p.h>

void metaPackValue(QSharedPointer<NetVariant> source, QVariant* destination);
void metaUnpackValue(QSharedPointer<NetVariant> destination, QVariant* source, NetVariantTypeEnum prefType);

QMetaObject *metaObjectFor(QSharedPointer<NetTypeInfo> typeInfo);

class NetValueMetaObject : public QAbstractDynamicMetaObject
{
public:
    NetValueMetaObject(QObject* value, QSharedPointer<NetReference> instance);

protected:
    int metaCall(QMetaObject::Call c, int id, void **a);

private:
    QObject *value;
    QSharedPointer<NetReference> instance;
};

#endif // NETVALUEMETAOBJECT_H

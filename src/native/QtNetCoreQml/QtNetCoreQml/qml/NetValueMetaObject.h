#ifndef NETVALUEMETAOBJECT_H
#define NETVALUEMETAOBJECT_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/qml/NetVariant.h>
#include <QtNetCoreQml/qml/NetValue.h>
#include <private/qobject_p.h>

void metaPackValue(QSharedPointer<NetVariant> source, QVariant* destination);
void metaUnpackValue(QSharedPointer<NetVariant> destination, QVariant* source, NetVariantTypeEnum prefType);

QMetaObject *metaObjectFor(QSharedPointer<NetTypeInfo> typeInfo);

class NetValueMetaObject : public QAbstractDynamicMetaObject
{
public:
    NetValueMetaObject(QObject* value, QSharedPointer<NetInstance> instance);

protected:
    int metaCall(QMetaObject::Call c, int id, void **a);

private:
    QObject *value;
    QSharedPointer<NetInstance> instance;
    int signalCount;
};

#endif // NETVALUEMETAOBJECT_H

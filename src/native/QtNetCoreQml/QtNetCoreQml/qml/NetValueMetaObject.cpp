#include <QtNetCoreQml/qml/NetValueMetaObject.h>
#include <QQmlEngine>
#include <QDebug>

void metaPackValue(QSharedPointer<NetVariant> source, QVariant* destination) {

}

void metaUnpackValue(NetVariant* destination, QVariant* source, NetVariantTypeEnum prefType) {

}

QMetaObject *metaObjectFor(QSharedPointer<NetTypeInfo> typeInfo)
{
    return NULL;
}

NetValueMetaObject::NetValueMetaObject(QObject *value,
                                       QSharedPointer<NetInstance> instance) :
    value(value),
    instance(instance),
    signalCount(0)
{

}

int NetValueMetaObject::metaCall(QMetaObject::Call c, int idx, void **a)
{
    return -1;
}

#ifndef NET_QML_META_H
#define NET_QML_META_H

#include "qtnetcoreqml_global.h"
#include <private/qobject_p.h>
#include <QDebug>

class NetTypeInfo;
class NetInstance;

QMetaObject *metaObjectFor(NetTypeInfo *typeInfo);

class GoValueMetaObject : public QAbstractDynamicMetaObject
{
public:
    GoValueMetaObject(QObject* value, NetInstance *instance);

protected:
    int metaCall(QMetaObject::Call c, int id, void **a);

private:
    QObject *value;
    NetInstance *instance;
};

#endif // NET_QML_META_H

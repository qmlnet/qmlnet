#ifndef NET_QML_META_H
#define NET_QML_META_H

#include "qtnetcoreqml_global.h"
#include <private/qobject_p.h>

QMetaObject *metaObjectFor(GoTypeInfo *typeInfo);

class GoValueMetaObject : public QAbstractDynamicMetaObject
{
public:
    GoValueMetaObject(QObject* value, GoAddr *addr, GoTypeInfo *typeInfo);

    void activatePropIndex(int propIndex);

protected:
    int metaCall(QMetaObject::Call c, int id, void **a);

private:
    QObject *value;
    GoAddr *addr;
    GoTypeInfo *typeInfo;
};

#endif // NET_QML_META_H

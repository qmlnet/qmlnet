#ifndef NET_QML_VALUE_H
#define NET_QML_VALUE_H

#include "qtnetcoreqml_global.h"
#include "net_qml_meta.h"
#include "net_type_info.h"

class GoValue : public QObject
{
    Q_OBJECT

public:
    NetAddr *addr;
    NetTypeInfo *typeInfo;

    GoValue(NetAddr *addr, NetTypeInfo *typeInfo, QObject *parent);
    virtual ~GoValue();

    void activate(int propIndex);

private:
    GoValueMetaObject *valueMeta;
};

#endif // NET_QML_VALUE_H

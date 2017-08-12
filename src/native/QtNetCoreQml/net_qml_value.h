#ifndef NET_QML_VALUE_H
#define NET_QML_VALUE_H

#include "qtnetcoreqml_global.h"
#include "net_qml_meta.h"
#include "net_type_info.h"

class NetValue : public QObject
{
    Q_OBJECT

public:
    NetAddr *addr;
    NetTypeInfo *typeInfo;

    NetValue(NetAddr *addr, NetTypeInfo *typeInfo, QObject *parent);
    virtual ~NetValue();

    void activate(int propIndex);

private:
    GoValueMetaObject *valueMeta;
};

#endif // NET_QML_VALUE_H

#ifndef NET_QML_VALUE_H
#define NET_QML_VALUE_H

#include "qtnetcoreqml_global.h"
#include "net_qml_meta.h"
#include "net_type_info.h"
#include "net_instance.h"

struct NetValueInterface
{
    virtual NetInstance* GetNetInstance() = 0;
};

Q_DECLARE_INTERFACE(NetValueInterface, "netcoreqml.NetValueInterface")

class NetValue : public QObject, NetValueInterface
{
    Q_OBJECT
    Q_INTERFACES(NetValueInterface)
public:
    NetValue(NetInstance *instance, QObject *parent);
    virtual ~NetValue();
    NetInstance* GetNetInstance();
private:
    NetInstance *instance;
    GoValueMetaObject *valueMeta;
};

#endif // NET_QML_VALUE_H

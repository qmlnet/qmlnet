#ifndef NET_QML_VALUE_H
#define NET_QML_VALUE_H

#include "qtnetcoreqml_global.h"
#include "net_qml_meta.h"
#include "net_type_info.h"
#include "net_instance.h"

class NetValue : public QObject
{
    Q_OBJECT

public:
    NetInstance *instance;

    NetValue(NetInstance *instance, QObject *parent);
    virtual ~NetValue();

private:
    GoValueMetaObject *valueMeta;
};

#endif // NET_QML_VALUE_H

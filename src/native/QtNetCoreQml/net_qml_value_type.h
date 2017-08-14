#ifndef NET_QML_VALUE_TYPE_H
#define NET_QML_VALUE_TYPE_H

#include "qtnetcoreqml_global.h"
#include "qobject.h"
#include "net_type_info_manager.h"
#include "net_qml_value.h"

template <int N>
class NetValueType : public NetValue
{
public:

    NetValueType()
        : NetValue(NetTypeInfoManager::CreateInstance(typeInfo), typeInfo, 0) {};

    static void init(NetTypeInfo *info)
    {
        typeInfo = info;
        static_cast<QMetaObject &>(staticMetaObject) = *metaObjectFor(typeInfo);
    };

    static NetTypeInfo *typeInfo;
    static QMetaObject staticMetaObject;
};

#endif // NET_QML_VALUE_TYPE_H

#include "net_qml_register_type.h"
#include "net_qml_value_type.h"
#include "qtestobject.h"
#include <QQmlApplicationEngine>

//#include "net_type_info.h"
//#include "net_type_info_manager.h"
//#include "net_qml_value.h"
//#include "net_qml_meta.h"

#define DEFINE_NETVALUETYPE(N) \
    template<> QMetaObject NetValueType<N>::staticMetaObject = QMetaObject(); \
    template<> NetTypeInfo *NetValueType<N>::typeInfo = 0;

DEFINE_NETVALUETYPE(1)

int registerNetType(QString &netType, QString &uri, int versionMajor, int versionMinor, QString& qmlName)
{
    if(!NetTypeInfoManager::isValidType(netType.toUtf8().data()))
        return -1;

    NetTypeInfo* typeInfo = NetTypeInfoManager::GetTypeInfo(netType.toUtf8().data());

    NetValueType<1>::init(typeInfo);

    return qmlRegisterType<NetValueType<1>>(uri.toUtf8().data(), versionMajor, versionMinor, qmlName.toUtf8().data());
}

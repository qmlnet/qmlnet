#include "net_qml_register_type.h"
#include "net_qml_value_type.h"

#include <QQmlApplicationEngine>

#include "net_type_info.h"
#include "net_qml_value.h"
#include "net_qml_meta.h"

#define DEFINE_GOVALUETYPE(N) \
    template<> QMetaObject GoValueType<N>::staticMetaObject = QMetaObject(); \
    template<> NetTypeInfo *GoValueType<N>::typeInfo = 0;

DEFINE_GOVALUETYPE(1)

int registerNetType(std::string netType, std::string uri, int versionMajor, int versionMinor, std::string qmlName)
{
    if(!NetTypeInfoManager::isValidType((char*)netType.c_str()))
        return -1;

    return qmlRegisterType<GoValueType<1>>(uri.c_str(), versionMajor, versionMinor, qmlName.c_str());
}

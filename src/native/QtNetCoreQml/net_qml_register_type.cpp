#include "net_qml_register_type.h"
#include "net_qml_value_type.h"

#include <QQmlApplicationEngine>

#include "net_qml_value.h"
#include "net_qml_meta.h"

#define DEFINE_GOVALUETYPE(N) \
    template<> QMetaObject GoValueType<N>::staticMetaObject = QMetaObject(); \
    template<> GoTypeInfo *GoValueType<N>::typeInfo = 0; \
    template<> GoTypeSpec_ *GoValueType<N>::typeSpec = 0;

DEFINE_GOVALUETYPE(1)

int registerNetType(std::string netType, std::string uri, int versionMajor, int versionMinor, std::string qmlName)
{
    GoValueType<1>::init(NULL, NULL);
    return qmlRegisterType<GoValueType<1>>(uri.c_str(), versionMajor, versionMinor, qmlName.c_str());
    return -1;

    //return
}

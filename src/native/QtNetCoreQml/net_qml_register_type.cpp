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

int registerNetType(std::string netType, std::string uri, int versionMajor, int versionMinor, std::string qmlName)
{
    qmlRegisterType<QTestObject>("test.io", 1, 0, "TestObject");
    //qmlRegisterType<QAnotherTestObject>("test.io", 1, 0, "AnotherTestObject");

    if(!NetTypeInfoManager::isValidType((char*)netType.c_str()))
        return -1;

    NetTypeInfo* typeInfo = NetTypeInfoManager::GetTypeInfo((char*)netType.c_str());

    NetInstance* instance = NetTypeInfoManager::CreateInstance(typeInfo);

    NetValueType<1>::init(typeInfo);

    return qmlRegisterType<NetValueType<1>>(uri.c_str(), versionMajor, versionMinor, qmlName.c_str());
}

%{
#include "net_test_helper.h"
%}
class NetTestHelper
{
public:
    static void RunQml(QQmlApplicationEngine* qmlApplicationEngine, QString& qml);
    static void RunQmlMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, QString& methodName, NetVariant* parameter, NetVariant* result);
};
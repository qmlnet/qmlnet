%{
#include "net_test_helper.h"
%}
class NetTestHelper
{
public:
    static void RunMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, NetMethodInfo* methodInfo, NetVariant* parameter, NetVariant* result);
};
#ifndef NETTESTHELPER_H
#define NETTESTHELPER_H

#include <QQmlApplicationEngine>
#include <net_variant.h>

class NetTestHelper
{
public:
    static void RunMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, NetMethodInfo* methodInfo, NetVariant* parameter, NetVariant* result);
};

#endif // NETTESTHELPER_H

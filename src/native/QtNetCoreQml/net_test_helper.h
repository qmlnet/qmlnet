#ifndef NETTESTHELPER_H
#define NETTESTHELPER_H

#include <QQmlApplicationEngine>
#include <net_variant.h>

class NetTestHelper
{
public:
    static void RunQml(QQmlApplicationEngine* qmlApplicationEngine, QString& qml);
    static void RunQmlMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, QString& methodName, NetVariant* parameter, NetVariant* result);
};

#endif // NETTESTHELPER_H

#ifndef NET_QQMLAPPLICATIONENGINE_H
#define NET_QQMLAPPLICATIONENGINE_H

#include <QmlNet.h>
#include <QQmlApplicationEngine>
#include <QSharedPointer>

struct QQmlApplicationEngineContainer {
    QSharedPointer<QQmlApplicationEngine> qmlEngine;
};

#endif // NET_QQMLAPPLICATIONENGINE_H

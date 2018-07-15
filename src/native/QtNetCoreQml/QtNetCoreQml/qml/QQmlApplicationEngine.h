#ifndef NET_QQMLAPPLICATIONENGINE_H
#define NET_QQMLAPPLICATIONENGINE_H

#include <QQmlApplicationEngine>
#include <QSharedPointer>

struct QQmlApplicationEngineContainer {
    QSharedPointer<QQmlApplicationEngine> qmlEngine;
};

#endif // NET_QQMLAPPLICATIONENGINE_H

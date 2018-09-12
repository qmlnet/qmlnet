#ifndef NET_QQMLAPPLICATIONENGINE_H
#define NET_QQMLAPPLICATIONENGINE_H

#include <QmlNet.h>
#include <QQmlApplicationEngine>

struct QQmlApplicationEngineContainer {
    QQmlApplicationEngine* qmlEngine;
    bool ownsEngine;
};

#endif // NET_QQMLAPPLICATIONENGINE_H

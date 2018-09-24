#ifndef NET_QQMLAPPLICATIONENGINE_H
#define NET_QQMLAPPLICATIONENGINE_H

#include <QmlNet.h>
#include <QQmlApplicationEngine>
#include <QmlNet/qml/JsNetObject.h>

struct QQmlApplicationEngineContainer {
    QQmlApplicationEngine* qmlEngine;
    JsNetObject* netObject;
    bool ownsEngine;
};

#endif // NET_QQMLAPPLICATIONENGINE_H

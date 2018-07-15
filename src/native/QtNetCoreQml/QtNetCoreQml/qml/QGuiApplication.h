#ifndef NET_QGUIAPPLICATION_H
#define NET_QGUIAPPLICATION_H

#include <QGuiApplication>
#include <QSharedPointer>

struct QGuiApplicationContainer {
    QSharedPointer<QGuiApplication> guiApp;
};

#endif // NET_QGUIAPPLICATION_H

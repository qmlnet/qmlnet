#ifndef NET_QAPPLICATION_H
#define NET_QAPPLICATION_H

#include <QmlNet.h>
#include <QApplication>
#include <QSharedPointer>

typedef void (*guiThreadTriggerCb)();

class GuiThreadContextTriggerCallback : public QObject {
    Q_OBJECT
public:
    GuiThreadContextTriggerCallback();
    guiThreadTriggerCb callback;
public slots:
    void trigger();
};

struct QApplicationContainer {
    int argCount;
    QList<QString> args;
    std::vector<char*> argsPointer;
    QApplication* app;
    bool ownsApp;
    QSharedPointer<GuiThreadContextTriggerCallback> callback;
};

#endif // NET_QAPPLICATION_H

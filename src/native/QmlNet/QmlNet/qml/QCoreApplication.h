#ifndef NET_QGUIAPPLICATION_H
#define NET_QGUIAPPLICATION_H

#include <QmlNet.h>
#include <QGuiApplication>
#include <QSharedPointer>

typedef void (*guiThreadTriggerCb)();

using guiThreadTriggerCb = void (*)();
using aboutToQuitCb = void (*)();

struct Q_DECL_EXPORT QCoreAppCallbacks {
    guiThreadTriggerCb guiThreadTrigger;
    aboutToQuitCb aboutToQuit;
};

class GuiThreadContextTriggerCallback : public QObject {
    Q_OBJECT
public:
    GuiThreadContextTriggerCallback();
    ~GuiThreadContextTriggerCallback();
    void setCallbacks(QCoreAppCallbacks* callbacks);
public slots:
    void trigger();
    void aboutToQuit();
private:
    QCoreAppCallbacks* _callbacks;
};

struct QGuiApplicationContainer {
    int argCount;
    QList<QString> args;
    std::vector<char*> argsPointer;
    QCoreApplication* app;
    bool ownsApp;
    QSharedPointer<GuiThreadContextTriggerCallback> callback;
};

#endif // NET_QGUIAPPLICATION_H

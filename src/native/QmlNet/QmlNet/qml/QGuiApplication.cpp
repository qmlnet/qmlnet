#include <QmlNet/qml/QGuiApplication.h>
#include <QGuiApplication>
#include <QmlNet.h>

GuiThreadContextTriggerCallback::GuiThreadContextTriggerCallback() :
    callback(NULL) {

}

void GuiThreadContextTriggerCallback::trigger() {
    if (callback) {
       callback();
    }
}

extern "C" {

Q_DECL_EXPORT QGuiApplicationContainer* qguiapplication_create() {
    QGuiApplicationContainer* result = new QGuiApplicationContainer();
    result->argCount = 0;
    result->guiApp = QSharedPointer<QGuiApplication>(new QGuiApplication(result->argCount, (char**)NULL, 0));
    result->callback = QSharedPointer<GuiThreadContextTriggerCallback>(new GuiThreadContextTriggerCallback());
    return result;
}

Q_DECL_EXPORT void qguiapplication_destroy(QGuiApplicationContainer* container) {
    delete container;
}

Q_DECL_EXPORT int qguiapplication_exec(QGuiApplicationContainer* container) {
    return container->guiApp->exec();
}

Q_DECL_EXPORT void qguiapplication_addTriggerCallback(QGuiApplicationContainer* container, guiThreadTriggerCb callback) {
    container->callback->callback = callback;
}

Q_DECL_EXPORT void qguiapplication_requestTrigger(QGuiApplicationContainer* container) {
    QMetaObject::invokeMethod(container->callback.data(), "trigger", Qt::QueuedConnection);
}

Q_DECL_EXPORT void qguiapplication_exit(QGuiApplicationContainer* container, int returnCode) {
    container->guiApp->exit(returnCode);
}

}

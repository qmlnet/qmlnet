#include <QtNetCoreQml/qml/QGuiApplication.h>
#include <QGuiApplication>

GuiThreadContextTriggerCallback::GuiThreadContextTriggerCallback() :
    callback(NULL) {

}

void GuiThreadContextTriggerCallback::trigger() {
    if (callback) {
       callback();
    }
}

extern "C" {

QGuiApplicationContainer* qguiapplication_create() {
    QGuiApplicationContainer* result = new QGuiApplicationContainer();
    result->argCount = 0;
    result->guiApp = QSharedPointer<QGuiApplication>(new QGuiApplication(result->argCount, (char**)NULL, 0));
    result->callback = QSharedPointer<GuiThreadContextTriggerCallback>(new GuiThreadContextTriggerCallback());
    return result;
}

void qguiapplication_destroy(QGuiApplicationContainer* container) {
    delete container;
}

int qguiapplication_exec(QGuiApplicationContainer* container) {
    return container->guiApp->exec();
}

void qguiapplication_addTriggerCallback(QGuiApplicationContainer* container, guiThreadTriggerCb callback) {
    container->callback->callback = callback;
}

void qguiapplication_requestTrigger(QGuiApplicationContainer* container) {
    QMetaObject::invokeMethod(container->callback.data(), "trigger", Qt::QueuedConnection);
}

}

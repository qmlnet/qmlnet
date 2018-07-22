#include <QmlNet/qml/QGuiApplication.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QGuiApplication>

GuiThreadContextTriggerCallback::GuiThreadContextTriggerCallback() :
    callback(nullptr) {
}

void GuiThreadContextTriggerCallback::trigger() {
    if (callback) {
       callback();
    }
}

extern "C" {

Q_DECL_EXPORT QGuiApplicationContainer* qguiapplication_create(NetVariantListContainer* argsContainer) {

    QGuiApplicationContainer* result = new QGuiApplicationContainer();

    // Build our args
    if(argsContainer != nullptr) {
        QSharedPointer<NetVariantList> args = argsContainer->list;
        for(int x = 0; x < args->count(); x++) {
            QByteArray arg = args->get(x)->getString().toLatin1();
            result->args.append(arg);
            char* cstr = nullptr;
            cstr = new char [arg.size()+1];
            strcpy(cstr, arg.data());
            result->argsPointer.push_back(cstr);
        }
        result->argCount = result->args.size();
    } else {
        result->argCount = 0;
    }

    result->guiApp = QSharedPointer<QGuiApplication>(new QGuiApplication(result->argCount, &result->argsPointer[0], 0));
    result->callback = QSharedPointer<GuiThreadContextTriggerCallback>(new GuiThreadContextTriggerCallback());

    return result;
}

Q_DECL_EXPORT void qguiapplication_destroy(QGuiApplicationContainer* container) {
    for (auto i : container->argsPointer) {
        delete i;
    }
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

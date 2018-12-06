#include <QmlNet/qml/QApplication.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QApplication>

GuiThreadContextTriggerCallback::GuiThreadContextTriggerCallback() :
    callback(nullptr) {
}

void GuiThreadContextTriggerCallback::trigger() {
    if (callback) {
       callback();
    }
}

extern "C" {

Q_DECL_EXPORT QApplicationContainer* qapplication_create(NetVariantListContainer* argsContainer, QApplication* existingApp) {
    QApplicationContainer* result = new QApplicationContainer();

    if (existingApp != nullptr) {
        result->ownsApp = false;
        result->app = existingApp;
    } else {
        result->ownsApp = true;
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
        result->app = new QApplication(result->argCount, &result->argsPointer[0], 0);
    }

    result->callback = QSharedPointer<GuiThreadContextTriggerCallback>(new GuiThreadContextTriggerCallback());

    return result;
}

Q_DECL_EXPORT void qapplication_destroy(QApplicationContainer* container) {
    for (auto i : container->argsPointer) {
        delete i;
    }
    container->callback.clear();
    if(container->ownsApp) {
        delete container->app;
    }
    delete container;
}

Q_DECL_EXPORT int qapplication_exec(QApplicationContainer* container) {
    return container->app->exec();
}

Q_DECL_EXPORT void qapplication_addTriggerCallback(QApplicationContainer* container, guiThreadTriggerCb callback) {
    container->callback->callback = callback;
}

Q_DECL_EXPORT void qapplication_requestTrigger(QApplicationContainer* container) {
    QMetaObject::invokeMethod(container->callback.data(), "trigger", Qt::QueuedConnection);
}

Q_DECL_EXPORT void qapplication_exit(QApplicationContainer* container, int returnCode) {
    container->app->exit(returnCode);
}

Q_DECL_EXPORT QApplication* qapplication_internalPointer(QApplicationContainer* container) {
    return container->app;
}

}

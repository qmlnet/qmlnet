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

Q_DECL_EXPORT QGuiApplicationContainer* qguiapplication_create(NetVariantListContainer* argsContainer, QGuiApplication* existingApp) {
    QGuiApplicationContainer* result = new QGuiApplicationContainer();

    if (existingApp != nullptr) {
        result->ownsGuiApp = false;
        result->guiApp = existingApp;
    } else {
        result->ownsGuiApp = true;
        // Build our args
        if(argsContainer != nullptr) {
            QSharedPointer<NetVariantList> args = argsContainer->list;
            for(int x = 0; x < args->count(); x++) {
                QByteArray arg = args->get(x)->getString().toLatin1();
                result->args.append(arg);
                char* cstr = new char [size_t(arg.size())+1];
                memcpy(cstr, arg.data(), size_t(arg.size())+1);
                result->argsPointer.push_back(cstr);
            }
            result->argCount = result->args.size();
        } else {
            result->argCount = 0;
        }
        result->guiApp = new QGuiApplication(result->argCount, &result->argsPointer[0], 0);
    }

    result->callback = QSharedPointer<GuiThreadContextTriggerCallback>(new GuiThreadContextTriggerCallback());

    return result;
}

Q_DECL_EXPORT void qguiapplication_destroy(QGuiApplicationContainer* container) {
    for (auto i : container->argsPointer) {
        delete i;
    }
    container->callback.clear();
    if(container->ownsGuiApp) {
        delete container->guiApp;
    }
    delete container;
}

Q_DECL_EXPORT int qguiapplication_exec(QGuiApplicationContainer* container) {
    return QGuiApplication::exec();
}

Q_DECL_EXPORT void qguiapplication_addTriggerCallback(QGuiApplicationContainer* container, guiThreadTriggerCb callback) {
    container->callback->callback = callback;
}

Q_DECL_EXPORT void qguiapplication_requestTrigger(QGuiApplicationContainer* container) {
    QMetaObject::invokeMethod(container->callback.data(), "trigger", Qt::QueuedConnection);
}

Q_DECL_EXPORT void qguiapplication_exit(QGuiApplicationContainer* container, int returnCode) {
    QGuiApplication::exit(returnCode);
}

Q_DECL_EXPORT QGuiApplication* qguiapplication_internalPointer(QGuiApplicationContainer* container) {
    return container->guiApp;
}

}

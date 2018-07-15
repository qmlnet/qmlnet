#include <QtNetCoreQml/qml/QGuiApplication.h>
#include <QGuiApplication>

extern "C" {

QGuiApplicationContainer* qguiapplication_create() {
    QGuiApplicationContainer* result = new QGuiApplicationContainer();
    result->argCount = 0;
    result->guiApp = QSharedPointer<QGuiApplication>(new QGuiApplication(result->argCount, (char**)NULL, 0));
    return result;
}

void qguiapplication_destroy(QGuiApplicationContainer* container) {
    delete container;
}

int qguiapplication_exec(QGuiApplicationContainer* container) {
    return container->guiApp->exec();
}

}

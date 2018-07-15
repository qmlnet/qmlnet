#include <QtNetCoreQml/qml/QGuiApplication.h>
#include <QGuiApplication>

extern "C" {

QGuiApplicationContainer* qguiapplication_create() {
    QGuiApplicationContainer* result = new QGuiApplicationContainer();
    result->guiApp = QSharedPointer(new QGuiApplication(0, NULL, 0));
    return result;
}

void qguiapplication_destroy(QGuiApplicationContainer* container) {
    delete container;
}

}

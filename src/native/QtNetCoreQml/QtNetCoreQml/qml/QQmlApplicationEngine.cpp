#include <QtNetCoreQml/qml/QQmlApplicationEngine.h>

extern "C" {

QQmlApplicationEngineContainer* qqmlapplicationengine_create() {
    QQmlApplicationEngineContainer* result = new QQmlApplicationEngineContainer();
    result->qmlEngine = QSharedPointer<QQmlApplicationEngine>(new QQmlApplicationEngine());
    return result;
}

void qqmlapplicationengine_destroy(QQmlApplicationEngineContainer* container) {
    delete container;
}

int qqmlapplicationengine_load(QQmlApplicationEngineContainer* container, LPWSTR path) {
    container->qmlEngine->load(QString::fromUtf16(path));
}

int qqmlapplicationengine_registerType(LPWSTR typeName, int versionMajor, int versionMinor, LPWSTR componentName) {
    return -1;
}

}

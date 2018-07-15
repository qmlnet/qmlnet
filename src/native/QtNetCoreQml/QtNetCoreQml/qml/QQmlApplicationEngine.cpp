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

}

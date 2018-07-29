#include <QmlNet/qml/NetTestHelper.h>
#include <QQmlComponent>
#include <QDebug>

extern "C" {

Q_DECL_EXPORT void net_test_helper_runQml(QQmlApplicationEngineContainer* qmlEngineContainer, LPWSTR qml) {
    QQmlComponent component(qmlEngineContainer->qmlEngine.data());
    QString qmlString = QString::fromUtf16((const char16_t*)qml);
    component.setData(qmlString.toUtf8(), QUrl());
    QObject *object = component.create();

    if(object == nullptr) {
        qWarning() << "Couldn't create qml object.";
        return;
    }

    delete object;
}

}

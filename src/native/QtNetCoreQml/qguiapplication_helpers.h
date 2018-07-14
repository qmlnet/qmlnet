#ifndef QGUIAPPLICATION_HELPERS_H
#define QGUIAPPLICATION_HELPERS_H

#include "qtnetcoreqml_global.h"
#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <vector>
#include <string>
#include <QDateTime>

class GuiThreadContextTriggerCallback : public QObject {
    Q_OBJECT
public:
    virtual ~GuiThreadContextTriggerCallback() {}
    virtual void onGuiThreadContextTrigger() {}
public slots:
    void trigger();
};

extern "C" {
QGuiApplication* new_QGuiApplication(std::vector<std::string> argv);

void QQmlApplicationEngine_loadFile(QQmlApplicationEngine* instance, std::string filePath);

void QGuiApplication_setGuiThreadContextTriggerCallback(QGuiApplication* instance, GuiThreadContextTriggerCallback* callback);
void QGuiApplication_requestGuiThreadContextTrigger(QGuiApplication* instance);


}
#endif // QGUIAPPLICATION_HELPERS_H

#include "qguiapplication_helpers.h"
#include <iostream>
#include <QDebug>
#include <QThread>

QGuiApplication* new_QGuiApplication(std::vector<std::string> argv)
{
    // this obviously will cause a memory leak, but only one should be created, per app instance.
    int* var = new int();
    *var = (int)argv.size();

    if(*var == 0)
        return new QGuiApplication(*var, NULL);

    std::vector<const char*>* newArgs = new std::vector<const char*>();
    for(int x = 0; x < argv.size(); x++)
    {
        newArgs->push_back(argv.at(x).c_str());
    }

    qDebug() << "args before:";
    for(unsigned int i=0; i<newArgs->size(); i++) {
        qDebug() << "[" << i << "]=" << newArgs->at(i);
    }

    QGuiApplication* app = new QGuiApplication(*var, (char**)&(newArgs->at(0)));

    qDebug() << "args after:";
    for(unsigned int i=0; i<newArgs->size(); i++) {
        qDebug() << "[" << i << "]=" << newArgs->at(i);
    }

    return app;
}

void QQmlApplicationEngine_loadFile(QQmlApplicationEngine* instance, std::string filePath)
{
    instance->load(QString::fromStdString(filePath));
}

GuiThreadContextTriggerCallback* s_guiThreadContextTriggerCallback = nullptr;

void QGuiApplication_setGuiThreadContextTriggerCallback(QGuiApplication*, GuiThreadContextTriggerCallback* callback) {
    s_guiThreadContextTriggerCallback = callback;
}

void QGuiApplication_requestGuiThreadContextTrigger(QGuiApplication* instance) {
    QMetaObject::invokeMethod(s_guiThreadContextTriggerCallback, "trigger", Qt::QueuedConnection);
}

void GuiThreadContextTriggerCallback::trigger() {
    onGuiThreadContextTrigger();
}

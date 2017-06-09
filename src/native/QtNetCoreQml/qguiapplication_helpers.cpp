#include "qguiapplication_helpers.h"
#include <iostream>
#include <QDebug>
#include <QThread>



QGuiApplication* new_QGuiApplication(std::vector<std::string> argv)
{
    // this obviously will cause a memory leak, but only one should be created, per app instance.
    int* var = new int();
    *var = (int)argv.size();

    qmlRegisterType<Message>("com.mycompany.messaging", 1, 0, "Message");

    if(*var == 0)
        return new QGuiApplication(*var, NULL);

    std::vector<const char*> newArgs;
    for(int x = 0; x < argv.size(); x++)
    {
        newArgs.push_back(argv.at(x).c_str());
    }

    auto app = new QGuiApplication(*var, (char**)&newArgs[0]);

    app->exec();

    return app;
}

void QQmlApplicationEngine_loadFile(QQmlApplicationEngine* instance, std::string filePath)
{
    instance->load(QString::fromUtf8("main.qml"));
}

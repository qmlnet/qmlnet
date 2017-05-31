#include "qtnetcoreqml.h"
#include <QDebug>
#include <QGuiApplication>
#include <QQmlApplicationEngine>

QtNetCoreQml::QtNetCoreQml()
{
}

void Test()
{
    int argc = 0;
    char *argv[1];

    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);
    QGuiApplication app(argc, argv);

    QQmlApplicationEngine engine;
    //engine.addImportPath("D:\\Git\\Github\\pauldotknopf\\net-core-qml\\tmp\\Test");
    engine.load(QUrl(QLatin1String("main.qml")));

    app.exec();
}

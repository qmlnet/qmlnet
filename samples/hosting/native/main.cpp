#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <Hosting/CoreHost.h>
#include <QDebug>
#include <QFileInfo>
#include <QDir>

static int runCallback(QGuiApplication* app, QQmlApplicationEngine* engine)
{
    engine->load(QUrl(QStringLiteral("qrc:/main.qml")));
    if (engine->rootObjects().isEmpty())
        return -1;

    return app->exec();
}

int main(int argc, char *argv[])
{
    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);
    QGuiApplication app(argc, argv);

    QQmlApplicationEngine engine;

    QString netDll = NET_ROOT;
    netDll.append(QDir::separator());
    netDll.append("NetHost.dll");

    CoreHost::RunContext runContext;
    runContext.hostFxrContext = CoreHost::findHostFxr();
    runContext.managedExe = netDll;
    // NOTE: You may set entry point to the current executable if
    // the .NET runtime is deployed side-by-side.
    runContext.entryPoint = runContext.hostFxrContext.dotnetRoot;
    runContext.entryPoint.append(CORECLR_DOTNET_EXE_NAME);

    return CoreHost::run(app,
        engine,
        runCallback,
        runContext);
}

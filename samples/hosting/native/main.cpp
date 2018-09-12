#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <Hosting/CoreHost.h>
#include <QDebug>
#include <QFileInfo>
#include <QDir>

static int runCallback(QGuiApplication* app, QQmlApplicationEngine* engine)
{
    // Phase 8
    // At this point, we are in Program.Main of the .NET program.
    // .NET should have registered all of it's types by now.

    // Load some QML files.
    // Maybe these QML files reference types registered in .NET.
    engine->load(QUrl(QStringLiteral("qrc:/main.qml")));
    if (engine->rootObjects().isEmpty())
        return -1;

    // Phase 9
    // Run the event loop.
    return app->exec();
}

int main(int argc, char *argv[])
{
    // Phase 1
    // Initialize Qt/QML like you would normally.
    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);
    QGuiApplication app(argc, argv);
    QQmlApplicationEngine engine;

    // Phase 2
    // Get the location to the managed exec
    QString netDll = NET_ROOT;
    netDll.append(QDir::separator());
    netDll.append("NetHost.dll");

    // Phase 3
    // Find .NET Core and it's libs/paths.
    CoreHost::RunContext runContext;
    runContext.hostFxrContext = CoreHost::findHostFxr();
    runContext.managedExe = netDll;
    // NOTE: You may set entry point to the current executable if
    // the .NET runtime is deployed side-by-side.
    runContext.entryPoint = runContext.hostFxrContext.dotnetRoot;
    runContext.entryPoint.append(CORECLR_DOTNET_EXE_NAME);

    // Phase 4
    // Run the .NET applciation.
    return CoreHost::run(app,
        engine,
        runCallback,
        runContext);
}

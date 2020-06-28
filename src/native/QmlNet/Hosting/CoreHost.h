#ifndef COREHOST_H
#define COREHOST_H

#include <QSharedPointer>
#include <Hosting/coreclrhost.h>
#include <QGuiApplication>
#include <QQmlApplicationEngine>

#ifdef _WIN32
#define CORECLR_DOTNET_EXE_NAME "dotnet.exe"
#else
#define CORECLR_DOTNET_EXE_NAME "dotnet"
#endif

class CoreHost : public QObject
{
    Q_OBJECT
public:
    struct HostFxrContext {
        bool success;
        QString dotnetRoot;
        QString hostFxrLib;
    };
    struct RunContext {
        HostFxrContext hostFxrContext;
        QString entryPoint;
        QString managedExe;
        QList<QString> argsPreAppend;
        QList<QString> args;
        QString nativeModule;
    };

    enum LoadHostFxrResult
    {
        Loaded,
        AlreadyLoaded,
        Failed
    };
    typedef int (*runCallback)(QGuiApplication* app, QQmlApplicationEngine* engine);

    static QList<QString> getPotientialDotnetRoots();
    static HostFxrContext findHostFxr();
    static int run(QGuiApplication& app, QQmlApplicationEngine& engine, runCallback runCallback, RunContext runContext);
};

#endif

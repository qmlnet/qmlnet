#include <Hosting/CoreHost.h>
#include <Hosting/coreclrhost.h>
#include <QFileInfo>
#include <QDir>
#include <QDirIterator>
#include <QVersionNumber>
#ifdef _WIN32
#include <Windows.h>
#else
#include <dlfcn.h>
#endif

#ifdef __APPLE__
#define HOSTFXR_DLL_NAME "libhostfxr.dylib"
#elif _WIN32
#define HOSTFXR_DLL_NAME "hostfxr.dll"
#else
#define HOSTFXR_DLL_NAME "libhostfxr.so"
#endif

static QString nativeModule;

static void* getExportedFunction(const char* symbolName) {
#ifdef _WIN32
    HMODULE library = GetModuleHandle(nativeModule.toLocal8Bit());
    FARPROC symbol = GetProcAddress(library, symbolName);
    return (void*)symbol;
#else
    void* dll = dlopen(nullptr, RTLD_LAZY);
    void* result = dlsym(dll, symbolName);
    dlclose(dll);
    return result;
#endif
}

QList<QString> CoreHost::getPotientialDotnetRoots()
{
    QList<QString> result;
#ifdef _WIN32
    result.push_back("C:\\Program Files\\dotnet");
#else
    result.push_back("/usr/local/share/dotnet");
    result.push_back("/usr/share/dotnet");
    result.push_back("/opt/dotnet");
#endif

    QByteArray dotnetRoot = qgetenv("DOTNET_ROOT");
    if(!dotnetRoot.isEmpty()) {
        // We are overriding the roots, forcing ourselves to look in a particular spot.
        result.clear();
        result.push_back(dotnetRoot);
    }

    return result;
}

CoreHost::HostFxrContext CoreHost::findHostFxr()
{
    HostFxrContext result;
    result.success = false;

    QList<QString> roots = getPotientialDotnetRoots();

    for(QString root : roots) {
        qDebug("looking for %s in root %s", HOSTFXR_DLL_NAME, qPrintable(root));
        if(!root.endsWith(QDir::separator())) {
            root.append(QDir::separator());
        }

        QFileInfo rootInfo(root);
        if(!rootInfo.exists()) {
            qDebug("%s doesn't exist", qPrintable(rootInfo.path()));
            continue;
        }

        QString host = root;
        host.append("host");
        host.append(QDir::separator());
        QFileInfo hostInfo(host);
        if(!hostInfo.exists()) {
            qDebug("%s doesn't exist", qPrintable(host));
            continue;
        }

        QString fxr = host;
        fxr.append("fxr");
        fxr.append(QDir::separator());
        QFileInfo fxrInfo(fxr);
        if(!fxrInfo.exists()) {
            qDebug("%s doesn't exist.", qPrintable(fxr));
            continue;
        }

        QString currentFxrLib;
        QVersionNumber currentFxrLibVersion;

        QDir fxrDir = fxrInfo.dir();
        fxrDir.setFilter(QDir::Dirs | QDir::NoDot | QDir::NoDotDot);
        QDirIterator it(fxrDir, QDirIterator::Subdirectories);
        while(it.hasNext()) {
            QString fxrVersion = it.next();
            QFileInfo fxrVersionInfo(fxrVersion);

            fxrVersion.append(QDir::separator());
            fxrVersion.append(HOSTFXR_DLL_NAME);

            QFileInfo fxrLibInfo(fxrVersion);
            if(!fxrLibInfo.exists()) {
                qDebug("%s doesn't exist", qPrintable(fxrLibInfo.absoluteFilePath()));
                continue;
            }

            QVersionNumber version = QVersionNumber::fromString(fxrVersionInfo.fileName());
            if(currentFxrLibVersion.isNull() || version > currentFxrLibVersion) {
                qDebug("found potentional file %s with version %s", qPrintable(fxrLibInfo.absoluteFilePath()), qPrintable((version.toString())));
                currentFxrLibVersion = version;
                currentFxrLib = fxrLibInfo.absoluteFilePath();
            } else {
                qDebug("ignore file %s with version %s", qPrintable(fxrLibInfo.absoluteFilePath()), qPrintable((version.toString())));
            }
        }

        if(!currentFxrLib.isEmpty()) {
            qDebug("returning hostfx lib: %s", qPrintable(currentFxrLib));
            result.success = true;
            result.hostFxrLib = currentFxrLib;
            result.dotnetRoot = root;
            return result;
        }
    }

    return result;
}

int CoreHost::run(QGuiApplication& app, QQmlApplicationEngine& engine, runCallback runCallback, RunContext runContext)
{
    nativeModule = runContext.nativeModule;

    QList<QString> execArgs;
    execArgs.push_back(runContext.entryPoint);
    execArgs.push_back("exec");
    execArgs.push_back(runContext.managedExe);

    QString appPtr;
    appPtr.sprintf("%llu", (quintptr)&app);
    QString enginePtr;
    enginePtr.sprintf("%llu", (quintptr)&engine);
    QString callbackPtr;
    callbackPtr.sprintf("%llu", (quintptr)runCallback);
    QString exportedSymbolPointer;
    exportedSymbolPointer.sprintf("%llu", (quintptr)getExportedFunction);

    execArgs.push_back(appPtr);
    execArgs.push_back(enginePtr);
    execArgs.push_back(callbackPtr);
    execArgs.push_back(exportedSymbolPointer);

    for (QString arg : runContext.args) {
        execArgs.push_back(arg);
    }

    std::vector<const CORECLR_CHAR_TYPE*> hostFxrArgs;

#ifdef _WIN32

    for (QString arg : execArgs) {
        hostFxrArgs.push_back(arg.utf16());
    }

#else

    QList<QByteArray> execArgs8bit;
    for (QString arg : execArgs) {
        execArgs8bit.push_back(arg.toLocal8Bit());
    }
    for (QByteArray arg : execArgs8bit) {
        hostFxrArgs.push_back(arg);
    }

#endif

    hostfxr_main_ptr hostfxr_main = nullptr;

#ifdef _WIN32

    HMODULE dll = LoadLibraryA(qPrintable(runContext.hostFxrContext.hostFxrLib));
    if(dll == nullptr) {
        qCritical("Couldn't load lib at %s", qPrintable(runContext.hostFxrContext.hostFxrLib));
        return LoadHostFxrResult::Failed;
    }

    hostfxr_main = reinterpret_cast<hostfxr_main_ptr>(GetProcAddress(dll, "hostfxr_main"));

#else

    void* dll = dlopen(qPrintable(runContext.hostFxrContext.hostFxrLib), RTLD_NOW | RTLD_LOCAL);
    if(dll == nullptr) {
        qCritical("Couldn't load lib at %s", qPrintable(runContext.hostFxrContext.hostFxrLib));
        return LoadHostFxrResult::Failed;
    }

    hostfxr_main = reinterpret_cast<hostfxr_main_ptr>(dlsym(dll, "hostfxr_main"));

#endif

    if(hostfxr_main == nullptr) {
        qCritical("Couldn't load 'hostfxr_main' from %s", qPrintable(runContext.hostFxrContext.hostFxrLib));
        return -1;
    }

    int result = hostfxr_main(static_cast<int>(hostFxrArgs.size()), &hostFxrArgs[0]);

#ifdef _WIN32
    FreeLibrary(dll);
#else
    dlclose(dll);
#endif

    return result;
}

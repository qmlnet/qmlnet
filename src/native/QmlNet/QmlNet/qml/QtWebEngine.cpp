#include <QmlNet/qml/QtWebEngine.h>
#ifdef QMLNET_WEBENGINE
#include <QtWebEngine>
#endif

extern "C" {

Q_DECL_EXPORT void qtwebebengine_initialize()
{
#ifdef QMLNET_WEBENGINE
    QtWebEngine::initialize();
#else
    qCritical("Qml.Net wasn't compiled with webengine.");
#endif
}

}

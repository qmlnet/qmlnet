#include <QmlNet/qml/QtWebEngine.h>
#include <QtWebEngine>

extern "C" {

Q_DECL_EXPORT void qtwebebengine_initialize()
{
    QtWebEngine::initialize();
}

}

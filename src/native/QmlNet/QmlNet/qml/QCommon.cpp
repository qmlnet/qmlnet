#include <QmlNet/qml/QCommon.h>
#include <QtGlobal>
#include <QString>
#include <QmlNetUtilities.h>

extern "C" {

Q_DECL_EXPORT bool qt_putenv(LPCSTR name, LPCSTR value)
{
    if(value == nullptr) {
        return qunsetenv(name);
    } else {
        return qputenv(name, value);
    }
}

Q_DECL_EXPORT QmlNetStringContainer* qt_getenv(LPCSTR name)
{
    QByteArray result = qgetenv(name);
    if(result.isNull()) {
        return nullptr;
    }
    QString string = QString(result);
    return createString(string);
}

Q_DECL_EXPORT QmlNetStringContainer* qt_version() {
    QString version(qVersion());
    return createString(version);
}

}

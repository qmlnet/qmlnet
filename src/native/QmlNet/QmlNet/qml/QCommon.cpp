#include <QmlNet/qml/QCommon.h>
#include <QString>

extern "C" {

Q_DECL_EXPORT bool qt_putenv(LPCSTR name, LPCSTR value)
{
    return qputenv(name, value);
}

Q_DECL_EXPORT LPCSTR qt_getenv(LPCSTR name)
{
    QByteArray result = qgetenv(name);
    if(result.isNull()) {
        return nullptr;
    }
    char* resultString = new char[static_cast<size_t>(result.size() + 1)];
    memcpy(resultString, result.data(), static_cast<size_t>(result.size() + 1));
    return resultString;
}

}

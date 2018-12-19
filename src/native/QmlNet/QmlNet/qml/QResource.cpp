#include <QmlNet/qml/QResource.h>
#include <QResource>
#include <QDir>

extern "C" {

Q_DECL_EXPORT bool qresource_registerResource(LPWSTR rccFileName, LPWSTR resourceRoot) {
    QString rccFileNameString = QString::fromUtf16(static_cast<const char16_t*>(rccFileName));
    QString resourceRootString = QString::fromUtf16(static_cast<const char16_t*>(resourceRoot));
    return QResource::registerResource(rccFileNameString, resourceRootString);
}

Q_DECL_EXPORT bool qresource_unregisterResource(LPWSTR rccFileName, LPWSTR resourceRoot) {
    QString rccFileNameString = QString::fromUtf16(static_cast<const char16_t*>(rccFileName));
    QString resourceRootString = QString::fromUtf16(static_cast<const char16_t*>(resourceRoot));
    return QResource::unregisterResource(rccFileNameString, resourceRootString);
}

}

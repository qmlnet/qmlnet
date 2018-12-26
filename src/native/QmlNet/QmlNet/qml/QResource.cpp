#include <QmlNet/qml/QResource.h>
#include <QResource>
#include <QDir>

extern "C" {

Q_DECL_EXPORT uchar qresource_registerResource(LPWSTR rccFileName, LPWSTR resourceRoot) {
    QString rccFileNameString = QString::fromUtf16(static_cast<const char16_t*>(rccFileName));
    QString resourceRootString = QString::fromUtf16(static_cast<const char16_t*>(resourceRoot));
    if(QResource::registerResource(rccFileNameString, resourceRootString)) {
        return 1;
    } else{
        return 0;
    }
}

Q_DECL_EXPORT uchar qresource_unregisterResource(LPWSTR rccFileName, LPWSTR resourceRoot) {
    QString rccFileNameString = QString::fromUtf16(static_cast<const char16_t*>(rccFileName));
    QString resourceRootString = QString::fromUtf16(static_cast<const char16_t*>(resourceRoot));
    if(QResource::unregisterResource(rccFileNameString, resourceRootString)) {
        return 1;
    } else {
        return 0;
    }
}

}

#include <QmlNet/qml/QResource.h>
#include <QResource>
#include <QDir>

extern "C" {

Q_DECL_EXPORT uchar qresource_registerResource(QChar* rccFileName, QChar* resourceRoot) {
    QString rccFileNameString(rccFileName);
    QString resourceRootString(resourceRoot);
    if(QResource::registerResource(rccFileNameString, resourceRootString)) {
        return 1;
    } else{
        return 0;
    }
}

Q_DECL_EXPORT uchar qresource_unregisterResource(QChar* rccFileName, QChar* resourceRoot) {
    QString rccFileNameString(rccFileName);
    QString resourceRootString(resourceRoot);
    if(QResource::unregisterResource(rccFileNameString, resourceRootString)) {
        return 1;
    } else {
        return 0;
    }
}

}

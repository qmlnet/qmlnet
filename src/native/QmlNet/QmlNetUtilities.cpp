#include <QmlNetUtilities.h>

extern "C" {

Q_DECL_EXPORT QmlNetStringContainer* createString(QString& value)
{
    if(value.isNull()) {
        return nullptr;
    }

    QmlNetStringContainer* result = new QmlNetStringContainer();
    result->container = new QString(value);
    result->data = result->container->utf16();

    return result;
}

Q_DECL_EXPORT void freeString(QmlNetStringContainer* container)
{
    if(container) {
        if(container->container) {
            delete container->container;
        }
        delete container;
    }
}

}


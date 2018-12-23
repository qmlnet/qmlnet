#ifndef QMLNETUTILITIES_H
#define QMLNETUTILITIES_H

#include <QmlNet.h>
#include <QString>

struct QmlNetStringContainer {
    QString* container;
    const ushort* data;
};

extern "C" {

Q_DECL_EXPORT QmlNetStringContainer* createString(const QString& value);
Q_DECL_EXPORT void freeString(QmlNetStringContainer* container);

}

#endif // QMLNETUTILITIES_H

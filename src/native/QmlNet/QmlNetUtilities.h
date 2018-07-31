#ifndef QMLNETUTILITIES_H
#define QMLNETUTILITIES_H

#include <QmlNet.h>
#include <QString>

struct QmlNetStringContainer {
    QString* container;
    const ushort* data;
};

extern "C" {

QmlNetStringContainer* createString(QString& value);
void freeString(QmlNetStringContainer* container);

}

#endif // QMLNETUTILITIES_H

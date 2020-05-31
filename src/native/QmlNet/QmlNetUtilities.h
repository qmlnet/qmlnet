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

/**
 * Creates a copy of the string suitable for returning to managed code.
 * Requires a [return:MarshalAs(LPWSTR)] on the managed side, and is 
 * only usable for *return values*.
 */
QChar *returnStringToDotNet(const QString &str);

/**
 * Takes ownership of a string returned by .NET via Marshal.StringToCoTaskMemUni.
 */
QString takeStringFromDotNet(QChar *str);

#endif // QMLNETUTILITIES_H

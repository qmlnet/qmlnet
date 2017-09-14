#ifndef NET_QML_REGISTER_TYPE_H
#define NET_QML_REGISTER_TYPE_H

#include <QString>

int registerNetType(QString &netType, QString &uri, int versionMajor, int versionMinor, QString &qmlName);

#endif // NET_QML_REGISTER_TYPE_H

#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QtNetCoreQml.h>
#include <QList>
#include <QString>
#include <QSharedPointer>

class NetTypeInfo {
public:
    NetTypeInfo(QString fullTypeName);
    ~NetTypeInfo();

    QString getFullTypeName();

private:
    QString _fullTypeName;
};

struct NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

#endif // NET_TYPE_INFO_H

#ifndef NETTYPEMANAGER_H
#define NETTYPEMANAGER_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetTypeInfo;

class NetTypeManager {
public:
    NetTypeManager();
    static QSharedPointer<NetTypeInfo> getTypeInfo(const QString& typeName);
private:
    static QMap<QString, QSharedPointer<NetTypeInfo>> types;
};


#endif // NETTYPEMANAGER_H

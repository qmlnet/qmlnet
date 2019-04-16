#ifndef NETTYPEMANAGER_H
#define NETTYPEMANAGER_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetTypeInfo;

class NetTypeManager {
public:
    NetTypeManager();
    static QSharedPointer<NetTypeInfo> getTypeInfo(const QString& typeName);
    static QSharedPointer<NetTypeInfo> getBaseType(QSharedPointer<NetTypeInfo> typeInfo);
private:
    static QMap<QString, QSharedPointer<NetTypeInfo>> types;
};


#endif // NETTYPEMANAGER_H

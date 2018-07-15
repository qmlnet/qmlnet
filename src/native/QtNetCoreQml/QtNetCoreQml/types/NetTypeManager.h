#ifndef NETTYPEMANAGER_H
#define NETTYPEMANAGER_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>

class NetTypeManager {
public:
    NetTypeManager();
    static QSharedPointer<NetTypeInfo> GetTypeInfo(QString typeName);
private:
    static QMap<QString, QSharedPointer<NetTypeInfo>> types;
};

#endif // NETTYPEMANAGER_H

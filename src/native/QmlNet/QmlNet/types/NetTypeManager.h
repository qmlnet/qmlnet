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
    static void registerAotObject(const QMetaObject* metaObject, int aotTypeId);
    static const QMetaObject* getAotMetaObject(int aotTypeId);
private:
    static QMap<QString, QSharedPointer<NetTypeInfo>> types;
    static QMap<int, const QMetaObject*> aotTypes;
};


#endif // NETTYPEMANAGER_H

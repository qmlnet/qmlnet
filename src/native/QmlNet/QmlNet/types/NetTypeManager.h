#ifndef NETTYPEMANAGER_H
#define NETTYPEMANAGER_H

#include <QmlNet.h>
#include <QSharedPointer>

using AotTypeInfoRegisterQml = int(*)(const char* uri, int versionMajor, int versionMinor, const char* qmlName);
using AotTypeInfoRegisterSingletonQml = int(*)(const char* uri, int versionMajor, int versionMinor, const char* typeName);

struct AotTypeInfo {
    const QMetaObject* metaObject;
    AotTypeInfoRegisterQml registerQmlFunction;
    AotTypeInfoRegisterSingletonQml registerQmlSingletonFunction;
};

class NetTypeInfo;

class NetTypeManager {
public:
    NetTypeManager();
    static QSharedPointer<NetTypeInfo> getTypeInfo(const QString& typeName);
    static QSharedPointer<NetTypeInfo> getBaseType(QSharedPointer<NetTypeInfo> typeInfo);
    static bool registerAotObject(int aotTypeId, const QMetaObject* metaObject, AotTypeInfoRegisterQml registerQmlFunction, AotTypeInfoRegisterSingletonQml registerSingletonQmlFunction);
    static AotTypeInfo* getAotInfo(int aotTypeId);
private:
    static QMap<QString, QSharedPointer<NetTypeInfo>> types;
    static QMap<int, AotTypeInfo*> aotTypes;
};


#endif // NETTYPEMANAGER_H

#include <QtNetCoreQml/types/NetTypeManager.h>
#include <QtNetCoreQml/types/Callbacks.h>
#include <QSharedPointer>

QMap<QString, QSharedPointer<NetTypeInfo>> NetTypeManager::types;

NetTypeManager::NetTypeManager() {
}

QSharedPointer<NetTypeInfo> NetTypeManager::getTypeInfo(QString typeName) {
    if(NetTypeManager::types.contains(typeName))
        return NetTypeManager::types.value(typeName);

    if(!isTypeValid(typeName)) {
        qWarning("Invalid type name: %s", qUtf8Printable(typeName));
        return QSharedPointer<NetTypeInfo>();
    }

    QSharedPointer<NetTypeInfo> typeInfo = QSharedPointer<NetTypeInfo>(new NetTypeInfo(typeName));
    NetTypeManager::types.insert(NetTypeManager::types.end(), typeName, typeInfo);

    buildTypeInfo(typeInfo);

    return typeInfo;
}

extern "C" {

NetTypeInfoContainer* type_manager_getTypeInfo(LPWSTR fullTypeName) {
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo(QString::fromUtf16(fullTypeName));
    if(typeInfo == NULL) {
        return NULL;
    }
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    return container;
}

}

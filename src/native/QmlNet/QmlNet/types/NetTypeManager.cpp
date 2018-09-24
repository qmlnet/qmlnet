#include <QmlNet/types/NetTypeManager.h>
#include <QmlNet/types/Callbacks.h>
#include <QSharedPointer>

using namespace QmlNet;

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

    createLazyTypeInfo(typeInfo);

    return typeInfo;
}

extern "C" {

Q_DECL_EXPORT NetTypeInfoContainer* type_manager_getTypeInfo(LPWSTR fullTypeName) {
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo(QString::fromUtf16((const char16_t*)fullTypeName));
    if(typeInfo == nullptr) {
        return nullptr;
    }
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    return container;
}

}

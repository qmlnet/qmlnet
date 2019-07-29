#include <QmlNet/types/NetTypeManager.h>
#include <QmlNet/types/Callbacks.h>
#include <QSharedPointer>
#include <QDebug>

using namespace QmlNet;

QMap<QString, QSharedPointer<NetTypeInfo>> NetTypeManager::types;
QMap<int, const QMetaObject*> NetTypeManager::aotTypes;

NetTypeManager::NetTypeManager() = default;

QSharedPointer<NetTypeInfo> NetTypeManager::getTypeInfo(const QString& typeName) {
    if(NetTypeManager::types.contains(typeName))
        return NetTypeManager::types.value(typeName);

    if(!isTypeValid(typeName)) {
        qWarning() << "Invalid type name:" << typeName;
        return QSharedPointer<NetTypeInfo>();
    }

    QSharedPointer<NetTypeInfo> typeInfo(new NetTypeInfo(typeName));
    NetTypeManager::types.insert(typeName, typeInfo);

    createLazyTypeInfo(typeInfo);

    return typeInfo;
}

QSharedPointer<NetTypeInfo> NetTypeManager::getBaseType(QSharedPointer<NetTypeInfo> typeInfo)
{
    auto baseType = typeInfo->getBaseType();
    if(baseType.isNull() || baseType.isEmpty()) {
        return nullptr;
    }
    return NetTypeManager::getTypeInfo(baseType);
}

void NetTypeManager::registerAotObject(const QMetaObject* metaObject, int aotTypeId)
{
    if(aotTypes.contains(aotTypeId)) {
        qWarning() << "Aot type id " << aotTypeId << " is already registered.";
        return;
    }

    aotTypes.insert(aotTypeId, metaObject);
}

const QMetaObject* NetTypeManager::getAotMetaObject(int aotTypeId)
{
    if(!aotTypes.contains(aotTypeId)) {
        qWarning() << "Aot type id " << aotTypeId << " doesn't exist.";
        return nullptr;
    }

    return aotTypes[aotTypeId];
}

extern "C" {

Q_DECL_EXPORT NetTypeInfoContainer* type_manager_getTypeInfo(LPWSTR fullTypeName) {
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo(QString::fromUtf16(static_cast<const char16_t*>(fullTypeName)));
    if(typeInfo == nullptr) {
        return nullptr;
    }
    NetTypeInfoContainer* container = new NetTypeInfoContainer();
    container->netTypeInfo = typeInfo;
    return container;
}

}

#include <QtNetCoreQml/types/NetPropertyInfo.h>

NetPropertyInfo::NetPropertyInfo(QSharedPointer<NetTypeInfo> parentType,
        QString name,
        QSharedPointer<NetTypeInfo> returnType,
        bool canRead,
        bool canWrite) :
    _parentType(parentType),
    _name(name),
    _returnType(returnType),
    _canRead(canRead),
    _canWrite(canWrite)
{

}

QSharedPointer<NetTypeInfo> NetPropertyInfo::getParentType()
{
    return _parentType;
}

QString NetPropertyInfo::getPropertyName()
{
    return _name;
}

QSharedPointer<NetTypeInfo> NetPropertyInfo::getReturnType()
{
    return _returnType;
}

bool NetPropertyInfo::canRead()
{
    return _canRead;
}

bool NetPropertyInfo::canWrite()
{
    return _canWrite;
}

extern "C" {

NetPropertyInfoContainer* property_info_create(NetTypeInfoContainer* parentType,
                                               LPWSTR name,
                                               NetTypeInfoContainer* returnType,
                                               bool canRead,
                                               bool canWrite) {
    NetPropertyInfoContainer* result = new NetPropertyInfoContainer();
    NetPropertyInfo* instance = new NetPropertyInfo(parentType->netTypeInfo,
                                                    QString::fromUtf16(name),
                                                    returnType->netTypeInfo,
                                                    canRead,
                                                    canWrite);
    result->property = QSharedPointer<NetPropertyInfo>(instance);
    return result;
}

void property_info_destroy(NetTypeInfoContainer* container) {
    delete container;
}

NetTypeInfoContainer* property_info_getParentType(NetPropertyInfoContainer* container) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->property->getParentType();
    return result;
}


LPWSTR property_info_getPropertyName(NetPropertyInfoContainer* container) {
    return (LPWSTR)container->property->getPropertyName().utf16();
}

NetTypeInfoContainer* property_info_getReturnType(NetPropertyInfoContainer* container) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = container->property->getReturnType();
    return result;
}

bool property_info_canRead(NetPropertyInfoContainer* container) {
    return container->property->canRead();
}

bool property_info_canWrite(NetPropertyInfoContainer* container) {
    return container->property->canWrite();
}

}

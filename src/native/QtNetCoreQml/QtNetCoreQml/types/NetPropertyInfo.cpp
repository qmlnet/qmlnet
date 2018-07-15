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

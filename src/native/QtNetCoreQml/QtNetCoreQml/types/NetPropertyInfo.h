#ifndef NET_TYPE_INFO_PROPERTY_H
#define NET_TYPE_INFO_PROPERTY_H

#include <QtNetCoreQml/types/NetTypeInfo.h>

class NetPropertyInfo {
public:
    NetPropertyInfo(QSharedPointer<NetTypeInfo> parentType,
                    QString name,
                    QSharedPointer<NetTypeInfo> returnType,
                    bool canRead,
                    bool canWrite);
    QSharedPointer<NetTypeInfo> getParentType();
    QString getPropertyName();
    QSharedPointer<NetTypeInfo> getReturnType();
    bool canRead();
    bool canWrite();
private:
    QSharedPointer<NetTypeInfo> _parentType;
    QString _name;
    QSharedPointer<NetTypeInfo> _returnType;
    bool _canRead;
    bool _canWrite;
};

struct NetPropertyInfoContainer {
    QSharedPointer<NetPropertyInfo> property;
};

#endif // NET_TYPE_INFO_PROPERTY_H

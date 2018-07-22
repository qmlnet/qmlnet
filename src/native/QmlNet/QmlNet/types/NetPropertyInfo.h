#ifndef NET_TYPE_INFO_PROPERTY_H
#define NET_TYPE_INFO_PROPERTY_H

#include <QmlNet/types/NetTypeInfo.h>

class NetPropertyInfo {
public:
    NetPropertyInfo(QSharedPointer<NetTypeInfo> parentType,
                    QString name,
                    QSharedPointer<NetTypeInfo> returnType,
                    bool canRead,
                    bool canWrite,
                    QSharedPointer<NetSignalInfo> notifySignal);
    QSharedPointer<NetTypeInfo> getParentType();
    QString getPropertyName();
    QSharedPointer<NetTypeInfo> getReturnType();
    bool canRead();
    bool canWrite();
    QSharedPointer<NetSignalInfo> getNotifySignal();
    void setNotifySignal(QSharedPointer<NetSignalInfo> signal);
private:
    QSharedPointer<NetTypeInfo> _parentType;
    QString _name;
    QSharedPointer<NetTypeInfo> _returnType;
    bool _canRead;
    bool _canWrite;
    QSharedPointer<NetSignalInfo> _notifySignal;
};

struct NetPropertyInfoContainer {
    QSharedPointer<NetPropertyInfo> property;
};

#endif // NET_TYPE_INFO_PROPERTY_H

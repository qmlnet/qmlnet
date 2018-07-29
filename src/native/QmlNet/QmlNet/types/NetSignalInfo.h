#ifndef NET_SIGNAL_INFO_METHOD_H
#define NET_SIGNAL_INFO_METHOD_H

#include <QmlNet.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QSharedPointer>

class NetSignalInfo {
public:
    NetSignalInfo(QSharedPointer<NetTypeInfo> parentType, QString name);
    QSharedPointer<NetTypeInfo> getParentType();
    QString getName();
    void addParameter(NetVariantTypeEnum type);
    int getParameterCount();
    NetVariantTypeEnum getParameter(int index);
    QString getSignature();
    QString getSlotSignature();
private:
    QSharedPointer<NetTypeInfo> _parentType;
    QString _name;
    QList<NetVariantTypeEnum> _parameters;
};

struct NetSignalInfoContainer {
    QSharedPointer<NetSignalInfo> signal;
};

#endif // NET_SIGNAL_INFO_METHOD_H

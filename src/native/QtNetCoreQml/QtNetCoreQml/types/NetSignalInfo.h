#ifndef NET_SIGNAL_INFO_METHOD_H
#define NET_SIGNAL_INFO_METHOD_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QSharedPointer>

class NetSignalInfo {
public:
    NetSignalInfo(QString name);
    QString getName();
    void addParameter(NetVariantTypeEnum type);
    uint getParameterCount();
    NetVariantTypeEnum getParameter(uint index);
private:
    QString _name;
    QList<NetVariantTypeEnum> _parameters;
};

struct NetSignalInfoContainer {
    QSharedPointer<NetSignalInfo> signal;
};

#endif // NET_SIGNAL_INFO_METHOD_H

#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QtNetCoreQml.h>
#include <QList>
#include <QString>
#include <QSharedPointer>

class NetTypeInfo {
public:
    NetTypeInfo(QString fullTypeName);
    ~NetTypeInfo();

    QString getFullTypeName();

    QString getClassName();
    void setClassName(QString className);

    NetVariantTypeEnum getPrefVariantType();
    void setPrefVariantType(NetVariantTypeEnum variantType);

private:
    QString _fullTypeName;
    QString _className;
    NetVariantTypeEnum _variantType;
};

struct NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

#endif // NET_TYPE_INFO_H

#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QtNetCoreQml.h>
#include <QList>
#include <QString>
#include <QSharedPointer>

class NetMethodInfo;

class NetTypeInfo {
public:
    NetTypeInfo(QString fullTypeName);
    ~NetTypeInfo();

    QString getFullTypeName();

    QString getClassName();
    void setClassName(QString className);

    NetVariantTypeEnum getPrefVariantType();
    void setPrefVariantType(NetVariantTypeEnum variantType);

    void addMethod(QSharedPointer<NetMethodInfo> methodInfo);
    uint getMethodCount();
    QSharedPointer<NetMethodInfo> getMethodInfo(uint index);

private:
    QString _fullTypeName;
    QString _className;
    NetVariantTypeEnum _variantType;
    QList<QSharedPointer<NetMethodInfo>> _methods;
};

struct NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

#endif // NET_TYPE_INFO_H

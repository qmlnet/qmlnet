#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QmlNet.h>
#include <QList>
#include <QString>
#include <QSharedPointer>

class NetMethodInfo;
class NetPropertyInfo;
class NetSignalInfo;

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

    void addProperty(QSharedPointer<NetPropertyInfo> property);
    uint getPropertyCount();
    QSharedPointer<NetPropertyInfo> getProperty(uint index);

    void addSignal(QSharedPointer<NetSignalInfo> signal);
    uint getSignalCount();
    QSharedPointer<NetSignalInfo> getSignal(uint index);

    QMetaObject* metaObject;

private:
    QString _fullTypeName;
    QString _className;
    NetVariantTypeEnum _variantType;
    QList<QSharedPointer<NetMethodInfo>> _methods;
    QList<QSharedPointer<NetPropertyInfo>> _properties;
    QList<QSharedPointer<NetSignalInfo>> _signals;
};

struct Q_DECL_EXPORT NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

#endif // NET_TYPE_INFO_H

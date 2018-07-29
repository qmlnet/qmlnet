#ifndef NET_TYPE_INFO_METHOD_H
#define NET_TYPE_INFO_METHOD_H

#include <QmlNet/types/NetTypeInfo.h>
#include <QSharedPointer>

class NetTypeInfo;

class NetMethodInfoArguement {
public:
    NetMethodInfoArguement(QString name, QSharedPointer<NetTypeInfo> type);
    QString getName();
    QSharedPointer<NetTypeInfo> getType();
private:
    QString _name;
    QSharedPointer<NetTypeInfo> _type;
};

class NetMethodInfo {
public:
    NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                  QString methodName,
                  QSharedPointer<NetTypeInfo> returnType);

    QString getMethodName();

    QSharedPointer<NetTypeInfo> getReturnType();

    void addParameter(QString name, QSharedPointer<NetTypeInfo> typeInfo);
    int getParameterCount();
    QSharedPointer<NetMethodInfoArguement> getParameter(int index);

    QString getSignature();

private:
    QSharedPointer<NetTypeInfo> _parentTypeInfo;
    QString _methodName;
    QSharedPointer<NetTypeInfo> _returnType;
    QList<QSharedPointer<NetMethodInfoArguement>> _parameters;
};

struct NetMethodInfoContainer {
    QSharedPointer<NetMethodInfo> method;
};

struct NetMethodInfoArguementContainer {
    QSharedPointer<NetMethodInfoArguement> methodArguement;
};

#endif // NET_TYPE_INFO_METHOD_H

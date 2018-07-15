#ifndef NET_TYPE_INFO_METHOD_H
#define NET_TYPE_INFO_METHOD_H

#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QSharedPointer>

class NetTypeInfo;

class NetMethodInfo {
public:
    NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                  QString methodName,
                  QSharedPointer<NetTypeInfo> returnType);
    QString GetMethodName();
    QSharedPointer<NetTypeInfo> GetReturnType();
private:
    QSharedPointer<NetTypeInfo> _parentTypeInfo;
    QString _methodName;
    QSharedPointer<NetTypeInfo> _returnType;
};

struct NetMethodInfoContainer {
    QSharedPointer<NetMethodInfo> methodInfo;
};

#endif // NET_TYPE_INFO_METHOD_H

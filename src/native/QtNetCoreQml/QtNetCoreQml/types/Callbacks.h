#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QSharedPointer>
#include <QString>

bool isTypeValid(QString type);

void releaseGCHandle(NetGCHandle* handle);

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

#endif // NET_TYPE_INFO_MANAGER_H

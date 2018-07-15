#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/types/NetInstance.h>
#include <QtNetCoreQml/qml/NetVariant.h>
#include <QSharedPointer>
#include <QString>

bool isTypeValid(QString type);

void releaseGCHandle(NetGCHandle* handle);

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

QSharedPointer<NetInstance> instantiateType(QSharedPointer<NetTypeInfo> type);

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetInstance> target, QSharedPointer<NetVariant> result);

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetInstance> target, QSharedPointer<NetVariant> value);

#endif // NET_TYPE_INFO_MANAGER_H

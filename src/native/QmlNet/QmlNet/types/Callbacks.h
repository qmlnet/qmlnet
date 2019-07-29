#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QmlNet.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetReference.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/qml/NetJsValue.h>
#include <QSharedPointer>
#include <QString>

namespace QmlNet {

bool isTypeValid(const QString& type);

void releaseNetReference(uint64_t objectId);

void releaseNetDelegateGCHandle(NetGCHandle* handle);

void createLazyTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

void loadTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

QSharedPointer<NetReference> instantiateType(QSharedPointer<NetTypeInfo> type, int aotTypeId);

void callComponentCompleted(QSharedPointer<NetReference> target);

void callObjectDestroyed(QSharedPointer<NetReference> target);

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, const QSharedPointer<NetVariant>& indexParameter, QSharedPointer<NetVariant> result);

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, const QSharedPointer<NetVariant>& indexParameter, QSharedPointer<NetVariant> value);

void invokeNetMethod(QSharedPointer<NetMethodInfo> method, QSharedPointer<NetReference> target, QSharedPointer<NetVariantList> parameters, const QSharedPointer<NetVariant>& result);

void gcCollect(int generation);

bool raiseNetSignals(QSharedPointer<NetReference> target, const QString& signalName, const QSharedPointer<NetVariantList>& parameters);

void awaitTask(QSharedPointer<NetReference> target, QSharedPointer<NetJSValue> successCallback, const QSharedPointer<NetJSValue>& failureCallback);

bool serializeNetToString(QSharedPointer<NetReference> instance, QSharedPointer<NetVariant> result);

void invokeDelegate(QSharedPointer<NetReference> del, QSharedPointer<NetVariantList> parameters);

}

#endif // NET_TYPE_INFO_MANAGER_H

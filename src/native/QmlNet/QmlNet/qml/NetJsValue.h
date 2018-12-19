#ifndef NETJSVALUE_H
#define NETJSVALUE_H

#include <QmlNet.h>
#include <QJSValue>
#include <QSharedPointer>

class NetVariant;
class NetVariantList;

class NetJSValue
{
public:
    NetJSValue(QJSValue jsValue);
    ~NetJSValue();
    QJSValue getJsValue();
    bool isCallable();
    bool isArray();
    QSharedPointer<NetVariant> call(const QSharedPointer<NetVariantList>& parameters);
    QSharedPointer<NetVariant> getProperty(const QString& propertyName);
    QSharedPointer<NetVariant> getItemAtIndex(quint32 arrayIndex);
    void setProperty(const QString& propertyName, const QSharedPointer<NetVariant>& variant);
    void setItemAtIndex(quint32 arrayIndex, const QSharedPointer<NetVariant>& variant);
private:
    QJSValue _jsValue;
};

struct NetJSValueContainer {
    QSharedPointer<NetJSValue> jsValue;
};

#endif // NETJSVALUE_H

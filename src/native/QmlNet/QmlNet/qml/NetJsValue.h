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
    QSharedPointer<NetVariant> call(QSharedPointer<NetVariantList> parameters);
    QSharedPointer<NetVariant> getProperty(QString propertyName);
private:
    QJSValue _jsValue;
};

struct NetJSValueContainer {
    QSharedPointer<NetJSValue> jsValue;
};

#endif // NETJSVALUE_H

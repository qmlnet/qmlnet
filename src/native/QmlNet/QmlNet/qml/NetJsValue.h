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
    bool isNumber();
    bool isString();
    QSharedPointer<NetVariant> call(QSharedPointer<NetVariantList> parameters);
    QSharedPointer<NetVariant> getProperty(QString propertyName);
    QSharedPointer<NetVariant> getItemAtIndex(quint32 arrayIndex);
    void setProperty(QString propertyName, QSharedPointer<NetVariant> variant);
    void setItemAtIndex(quint32 arrayIndex, QSharedPointer<NetVariant> variant);
    double toNumber();
    QString toString();
private:
    QJSValue _jsValue;
};

struct NetJSValueContainer {
    QSharedPointer<NetJSValue> jsValue;
};

#endif // NETJSVALUE_H

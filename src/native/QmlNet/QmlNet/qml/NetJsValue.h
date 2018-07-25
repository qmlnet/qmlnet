#ifndef NETJSVALUE_H
#define NETJSVALUE_H

#include <QmlNet.h>
#include <QJSValue>
#include <QSharedPointer>

class NetJSValue
{
public:
    NetJSValue(QJSValue jsValue);
    ~NetJSValue();
    bool isCallable();
private:
    QJSValue _jsValue;
};

struct NetJSValueContainer {
    QSharedPointer<NetJSValue> jsValue;
};

#endif // NETJSVALUE_H

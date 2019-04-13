#ifndef JSNETOBJECT_H
#define JSNETOBJECT_H

#include <QmlNet.h>
#include <QObject>
#include <QJSValue>
#include <QVariant>

class JsNetObject : public QObject
{
    Q_OBJECT
public:
    JsNetObject();
    Q_INVOKABLE QString serialize(const QJSValue& value);
    Q_INVOKABLE QVariant cancelTokenSource();
    Q_INVOKABLE void gcCollect(int maxGeneration = 0);
    Q_INVOKABLE QVariant toListModel(const QJSValue& value);
    Q_INVOKABLE QVariant listForEach(const QJSValue& value, QJSValue callback);
    Q_INVOKABLE void toJsArray();
    Q_INVOKABLE void await(const QJSValue& task, const QJSValue& successCallback, const QJSValue& failureCallback = QJSValue());
};

#endif // JSNETOBJECT_H

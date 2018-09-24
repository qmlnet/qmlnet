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
    Q_INVOKABLE QString serialize(QJSValue value);
    Q_INVOKABLE QVariant cancelTokenSource();
    Q_INVOKABLE void gcCollect(int maxGeneration = 0);
    Q_INVOKABLE QVariant toListModel(QJSValue value);
    Q_INVOKABLE void toJsArray();
    Q_INVOKABLE void await(QJSValue task, QJSValue successCallback, QJSValue failureCallback = QJSValue());
};

#endif // JSNETOBJECT_H

#include <QmlNet/qml/NetJsValue.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QDebug>
#include <QJSEngine>
#include <private/qjsvalue_p.h>

NetJSValue::NetJSValue(QJSValue jsValue) :
    _jsValue(jsValue)
{

}

NetJSValue::~NetJSValue()
{
    _jsValue.isCallable();
}

bool NetJSValue::isCallable()
{
    return _jsValue.isCallable();
}

QSharedPointer<NetVariant> NetJSValue::call(QSharedPointer<NetVariantList> parameters)
{
    QJSValueList jsValueList;
    if(parameters != nullptr) {
        for(int x = 0; x < parameters->count(); x++) {
            QVariant qVariant = parameters->get(x)->getVariant();
            QJSEngine* engine = QJSValuePrivate::engine(&_jsValue)->jsEngine();
            jsValueList.append(engine->toScriptValue<QVariant>(qVariant));
        }
    }

    _jsValue.call(jsValueList);
    return nullptr;
}

extern "C" {

Q_DECL_EXPORT void net_js_value_destroy(NetJSValueContainer* jsValueContainer) {
    delete jsValueContainer;
}

Q_DECL_EXPORT bool net_js_value_isCallable(NetJSValueContainer* jsValueContainer) {
    return jsValueContainer->jsValue->isCallable();
}

Q_DECL_EXPORT NetVariantListContainer* net_js_value_call(NetJSValueContainer* jsValueContainer, NetVariantListContainer* parametersContainer) {
    QSharedPointer<NetVariantList> parameters;
    if(parametersContainer != nullptr) {
        parameters = parametersContainer->list;
    }
    jsValueContainer->jsValue->call(parameters);
    return nullptr; // TODO, return the result.
}

}

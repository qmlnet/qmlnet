#include "NetJsValue.h"

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

extern "C" {

Q_DECL_EXPORT void net_js_value_destroy(NetJSValueContainer* jsValueContainer) {
    delete jsValueContainer;
}

Q_DECL_EXPORT bool net_js_value_isCallable(NetJSValueContainer* jsValueContainer) {
    return jsValueContainer->jsValue->isCallable();
}

}

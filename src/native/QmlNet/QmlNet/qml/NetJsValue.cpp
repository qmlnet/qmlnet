#include <QmlNet/qml/NetJsValue.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetJsValue.h>
#include <QDebug>
#include <QJSEngine>
#include <utility>

NetJSValue::NetJSValue(QJSValue jsValue) :
    _jsValue(std::move(jsValue))
{

}

NetJSValue::~NetJSValue()
{
    _jsValue.isCallable();
}

QJSValue NetJSValue::getJsValue()
{
    return _jsValue;
}

bool NetJSValue::isCallable() const
{
    return _jsValue.isCallable();
}

bool NetJSValue::isArray() const
{
    return _jsValue.isArray();
}

QSharedPointer<NetVariant> NetJSValue::call(const QSharedPointer<NetVariantList>& parameters)
{
    QJSValueList jsValueList;
    if(parameters != nullptr) {
        for(int x = 0; x < parameters->count(); x++) {
            QSharedPointer<NetVariant> netVariant = parameters->get(x);
            jsValueList.append(netVariant->toQJSValue(_jsValue.engine()));
        }
    }

    return NetVariant::fromQJSValue(_jsValue.call(jsValueList));
}

QSharedPointer<NetVariant> NetJSValue::getProperty(const QString& propertyName)
{
    QJSValue property = _jsValue.property(propertyName);
    return NetVariant::fromQJSValue(property);
}

QSharedPointer<NetVariant> NetJSValue::getItemAtIndex(quint32 arrayIndex)
{
    QJSValue property = _jsValue.property(arrayIndex);
    return NetVariant::fromQJSValue(property);
}

void NetJSValue::setProperty(const QString& propertyName, const QSharedPointer<NetVariant>& variant)
{
    QJSValue value = QJSValue::NullValue;
    if(variant != nullptr) {
        value = variant->toQJSValue(_jsValue.engine());
    }
    _jsValue.setProperty(propertyName, value);
}

void NetJSValue::setItemAtIndex(quint32 arrayIndex, const QSharedPointer<NetVariant>& variant)
{
    QJSValue value = QJSValue::NullValue;
    if(variant != nullptr) {
        value = variant->toQJSValue(_jsValue.engine());
    }
    _jsValue.setProperty(arrayIndex, value);
}

extern "C" {

Q_DECL_EXPORT void net_js_value_destroy(NetJSValueContainer* jsValueContainer) {
    delete jsValueContainer;
}

Q_DECL_EXPORT bool net_js_value_isCallable(NetJSValueContainer* jsValueContainer) {
    // Weird bug here, maybe someone can help explain?
    // If I were to make this method return "jsValueContainer->jsValue->isCallable()"
    // directly, sometimes, in .NET the bool returned will be wrong.
    // However, if I first set the bool on the stack, and return it, it (seems?)
    // to always work.
    // Also, this only seemed to happen after updating to Qt 5.12.
    // I'm not sure what is going on here...
    auto result = jsValueContainer->jsValue->isCallable();
    return result;
}

Q_DECL_EXPORT bool net_js_value_isArray(NetJSValueContainer* jsValueContainer) {
    return jsValueContainer->jsValue->isArray();
}

Q_DECL_EXPORT NetVariantContainer* net_js_value_call(NetJSValueContainer* jsValueContainer, NetVariantListContainer* parametersContainer) {
    QSharedPointer<NetVariantList> parameters;
    if(parametersContainer != nullptr) {
        parameters = parametersContainer->list;
    }
    QSharedPointer<NetVariant> result = jsValueContainer->jsValue->call(parameters);
    if(result != nullptr) {
        return new NetVariantContainer{result};
    }
    return nullptr;
}

Q_DECL_EXPORT NetVariantContainer* net_js_value_getProperty(NetJSValueContainer* jsValueContainer, LPWSTR propertyName) {
    QSharedPointer<NetVariant> result = jsValueContainer->jsValue->getProperty(QString::fromUtf16(static_cast<const char16_t*>(propertyName)));
    if(result == nullptr) {
        return nullptr;
    }
    return new NetVariantContainer{result};
}

Q_DECL_EXPORT NetVariantContainer* net_js_value_getItemAtIndex(NetJSValueContainer* jsValueContainer, quint32 arrayIndex) {
    QSharedPointer<NetVariant> result = jsValueContainer->jsValue->getItemAtIndex(arrayIndex);
    if(result == nullptr) {
        return nullptr;
    }
    return new NetVariantContainer{result};
}

Q_DECL_EXPORT void net_js_value_setProperty(NetJSValueContainer* jsValueContainer, LPWSTR propertyName, NetVariantContainer* valueContainer) {
    QSharedPointer<NetVariant> value;
    if(valueContainer != nullptr) {
        value = valueContainer->variant;
    }
    jsValueContainer->jsValue->setProperty(QString::fromUtf16(static_cast<const char16_t*>(propertyName)), value);
}

Q_DECL_EXPORT void net_js_value_setItemAtIndex(NetJSValueContainer* jsValueContainer, quint32 arrayIndex, NetVariantContainer* valueContainer) {
    QSharedPointer<NetVariant> value;
    if(valueContainer != nullptr) {
        value = valueContainer->variant;
    }
    jsValueContainer->jsValue->setItemAtIndex(arrayIndex, value);
}

}

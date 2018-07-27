#include <QmlNet/qml/NetJsValue.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetJsValue.h>
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

QJSValue NetJSValue::getJsValue()
{
    return _jsValue;
}

bool NetJSValue::isCallable()
{
    return _jsValue.isCallable();
}

QSharedPointer<NetVariant> NetJSValue::call(QSharedPointer<NetVariantList> parameters)
{
    QV4::ExecutionEngine* engine = QJSValuePrivate::engine(&_jsValue);

    QJSValueList jsValueList;
    if(parameters != nullptr) {
        for(int x = 0; x < parameters->count(); x++) {
            QSharedPointer<NetVariant> netVariant = parameters->get(x);
            switch(netVariant->getVariantType()) {
            case NetVariantTypeEnum_Object: {
                QSharedPointer<NetReference> netReference = netVariant->getNetReference();
                NetValue* netValue = NetValue::forInstance(netReference);
                jsValueList.append(engine->jsEngine()->newQObject(netValue));
                break;
            }
            case NetVariantTypeEnum_JSValue: {
                QSharedPointer<NetJSValue> netJsValue = netVariant->getJsValue();
                jsValueList.append(netJsValue->getJsValue());
                break;
            }
            default: {
                QVariant qVariant = parameters->get(x)->getVariant();
                jsValueList.append(engine->jsEngine()->toScriptValue<QVariant>(qVariant));
                break;
            }
            }

        }
    }

    QJSValue methodResult = _jsValue.call(jsValueList);
    QSharedPointer<NetVariant> result;

    if(methodResult.isNull() || methodResult.isUndefined()) {
        // Do nothing, return null;
    }
    else if(methodResult.isQObject()) {
        QObject* qObject = methodResult.toQObject();
        NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);
        if(!netValue) {
            qWarning() << "Return type must be a JS type/object, or a .NET object.";
        } else {
            result = QSharedPointer<NetVariant>(new NetVariant());
            result->setNetReference(netValue->getNetReference());
        }
    }
    else if(methodResult.isObject()) {
        result = QSharedPointer<NetVariant>(new NetVariant());
        result->setJsValue(QSharedPointer<NetJSValue>(new NetJSValue(methodResult)));
    }
    else if(methodResult.isQObject()) {
        QObject* qObject = methodResult.toQObject();
        NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);
        if(!netValue) {
            qWarning() << "Return type must be a JS type/object, or a .NET object.";
        } else {
            result = QSharedPointer<NetVariant>(new NetVariant());
            result->setNetReference(netValue->getNetReference());
        }
    }
    else {
        result = QSharedPointer<NetVariant>(new NetVariant());
        QVariant variant = methodResult.toVariant();
        result->setVariant(variant);
    }

    return result;
}

extern "C" {

Q_DECL_EXPORT void net_js_value_destroy(NetJSValueContainer* jsValueContainer) {
    delete jsValueContainer;
}

Q_DECL_EXPORT bool net_js_value_isCallable(NetJSValueContainer* jsValueContainer) {
    return jsValueContainer->jsValue->isCallable();
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

}

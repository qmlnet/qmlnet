#include <QmlNet/qml/JsNetObject.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/types/NetTypeManager.h>
#include <private/qv4qobjectwrapper_p.h>
#include <QDebug>

using namespace QV4;

DEFINE_OBJECT_VTABLE(NetObject);

void Heap::NetObject::init() {
    Scope scope(internalClass->engine);
    ScopedObject o(scope, this);
    o->defineDefaultProperty(QStringLiteral("gcCollect"), QV4::NetObject::method_gccollect);
    o->defineDefaultProperty(QStringLiteral("await"), QV4::NetObject::method_await);
    o->defineDefaultProperty(QStringLiteral("cancelTokenSource"), QV4::NetObject::method_cancelTokenSource);
    o->defineDefaultProperty(QStringLiteral("serialize"), QV4::NetObject::method_serialize);
}

#if QT_VERSION < QT_VERSION_CHECK(5, 11, 0)

void NetObject::method_gccollect(const BuiltinFunction *, Scope &scope, CallData *callData) {
    int maxGeneration = 0;
    if(callData->argc > 0) {
        maxGeneration = callData->args[0].toNumber();
    }
    gcCollect(maxGeneration);
    scope.result = QV4::Encode::undefined();
}

void NetObject::method_await(const BuiltinFunction *, Scope &scope, CallData *callData) {
    scope.result = QV4::Encode::undefined();

    if (callData->argc != 2 && callData->argc != 3) {
        qWarning() << "Invalid number of parameters passed to Net.await(task, callback)";
        return;
    }

    ScopedValue task(scope, callData->args[0]);
    ScopedValue successCallback(scope, callData->args[1]);
    ScopedValue failureCallback(scope);

    if(callData->argc == 3) {
        failureCallback = ScopedValue(scope, callData->args[2]);
    }

    if(task->isNullOrUndefined()) {
        qWarning() << "No task for Net.await(task, callback)";
        return;
    }

    if(successCallback->isNullOrUndefined()) {
        qWarning() << "No callback for Net.await(task, callback)";
        return;
    }

    QJSValue taskJsValue(scope.engine, task->asReturnedValue());
    QJSValue callbackJsValue(scope.engine, successCallback->asReturnedValue());
    QJSValue failureJsValue(scope.engine, failureCallback->asReturnedValue());

    QObject* qObject = taskJsValue.toQObject();
    NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);

    if(!netValue) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return;
    }

    // Send the method to .NET, await the task, and call the callback.
    awaitTask(netValue->getNetReference(),
              QSharedPointer<NetJSValue>(new NetJSValue(callbackJsValue)),
              failureJsValue.isNull()
                ? QSharedPointer<NetJSValue>(nullptr)
                : QSharedPointer<NetJSValue>(new NetJSValue(failureJsValue)));
}

void NetObject::method_cancelTokenSource(const BuiltinFunction *, Scope &scope, CallData *callData) {
    Q_UNUSED(callData);
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo("System.Threading.CancellationTokenSource");
    if(typeInfo == nullptr) {
        qWarning() << "Couldn't get cancellation token type for platform, please file a bug";
        scope.result = QV4::Encode::undefined();
        return;
    }
    QSharedPointer<NetReference> netReference = instantiateType(typeInfo);
    if(netReference == nullptr) {
        qWarning() << "Couldn't create cancellation token for platform, please file a bug";
        scope.result = QV4::Encode::undefined();
        return;
    }
    NetValue* netValue = NetValue::forInstance(netReference);
    scope.result = QV4::QObjectWrapper::wrap(scope.engine, netValue);
}

void NetObject::method_serialize(const BuiltinFunction *, Scope &scope, CallData *callData)
{
    qDebug("TESTdd");

    if(callData->argc != 1) {
        THROW_GENERIC_ERROR("Net.serialize(): Missing instance parameter");
    }

    ScopedValue instance(scope, callData->args[0]);
    if(instance->isNullOrUndefined()) {
        THROW_GENERIC_ERROR("Net.serialize(): Instance parameter must not be null or undefined");
    }

    QJSValue instanceJsValue(scope.engine, instance->asReturnedValue());
    QSharedPointer<NetVariant> value = NetVariant::fromQJSValue(instanceJsValue);
    if(value->getVariantType() != NetVariantTypeEnum_Object) {
        THROW_GENERIC_ERROR("Net.serialize(): Parameter is not a .NET object");
    }

    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    bool serializationResult = serializeNetToString(value->getNetReference(), result);
    if(!serializationResult) {
        THROW_GENERIC_ERROR("Net.serialize(): Could not serialize object.");
    }

    scope.result = scope.engine->newString(result->getString());
}

#else

ReturnedValue NetObject::method_gccollect(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc) {
    Q_UNUSED(b);
    Q_UNUSED(thisObject);
    int maxGeneration = 0;
    if(argc > 0) {
        maxGeneration = static_cast<int>(argv[0].toNumber());
    }
    gcCollect(maxGeneration);
    return Encode::undefined();
}

ReturnedValue NetObject::method_await(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc) {
    QV4::Scope scope(b);

    if(argc != 2 && argc != 3) {
        qWarning() << "Invalid number of parameters passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    QV4::ScopedValue task(scope, argv[0]);
    QV4::ScopedValue successCallback(scope, argv[1]);
    QV4::ScopedValue failureCallback(scope, Encode::null());

    if(argc == 3) {
        // We are passing in a failure callback
        failureCallback = argv[2];
    }

    if(task->isNullOrUndefined()) {
        qWarning() << "No task for Net.await(task, callback)";
        return Encode::undefined();
    }

    if(successCallback->isNullOrUndefined()) {
        qWarning() << "No callback for Net.await(task, callback)";
        return Encode::undefined();
    }

    QJSValue taskJsValue(scope.engine, task->asReturnedValue());
    QJSValue successCallbackJsValue(scope.engine, successCallback->asReturnedValue());
    QJSValue failureCallbackJsValue(scope.engine, failureCallback->asReturnedValue());

    if(!taskJsValue.isQObject()) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    if(!successCallbackJsValue.isCallable()) {
        qWarning() << "Invalid function passed for success callback";
        return Encode::undefined();
    }

    if(!failureCallbackJsValue.isNull() && !failureCallbackJsValue.isCallable()) {
        qWarning() << "Invalid function passed for failure callback";
        return Encode::undefined();
    }

    QObject* qObject = taskJsValue.toQObject();
    NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);

    if(!netValue) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    // Send the method to .NET, await the task, and call the callback.
    awaitTask(netValue->getNetReference(),
              QSharedPointer<NetJSValue>(new NetJSValue(successCallbackJsValue)),
              failureCallbackJsValue.isNull()
                ? QSharedPointer<NetJSValue>(nullptr)
                : QSharedPointer<NetJSValue>(new NetJSValue(failureCallbackJsValue)));

    return Encode::undefined();
}

ReturnedValue NetObject::method_cancelTokenSource(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc) {
    QV4::Scope scope(b);
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo("System.Threading.CancellationTokenSource");
    if(typeInfo == nullptr) {
        qWarning() << "Couldn't get cancellation token type for platform, please file a bug";
        return Encode::undefined();
    }
    QSharedPointer<NetReference> netReference = instantiateType(typeInfo);
    if(netReference == nullptr) {
        qWarning() << "Couldn't create cancellation token for platform, please file a bug";
        return Encode::undefined();
    }
    NetValue* netValue = NetValue::forInstance(netReference);
    return QV4::QObjectWrapper::wrap(scope.engine, netValue);
}

ReturnedValue NetObject::method_serialize(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc)
{
    QV4::Scope scope(b);
    if(argc != 1) {
        THROW_GENERIC_ERROR("Net.serialize(): Missing instance parameter");
    }
    QV4::ScopedValue instance(scope, argv[0]);
    if(instance->isNullOrUndefined()) {
        THROW_GENERIC_ERROR("Net.serialize(): Instance parameter must not be null or undefined");
    }
    QJSValue instanceJsValue(scope.engine, instance->asReturnedValue());
    QSharedPointer<NetVariant> value = NetVariant::fromQJSValue(instanceJsValue);
    if(value->getVariantType() != NetVariantTypeEnum_Object) {
        THROW_GENERIC_ERROR("Net.serialize(): Parameter is not a .NET object");
    }
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    bool serializationResult = serializeNetToString(value->getNetReference(), result);
    if(!serializationResult) {
        THROW_GENERIC_ERROR("Net.serialize(): Could not serialize object.");
    }

    return Encode(scope.engine->newString(result->getString()));
}

#endif

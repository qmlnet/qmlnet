#include <QmlNet/qml/JsNetObject.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/qml/NetValue.h>
#include <QDebug>

using namespace QV4;

DEFINE_OBJECT_VTABLE(NetObject);

void Heap::NetObject::init() {
    Scope scope(internalClass->engine);
    ScopedObject o(scope, this);
    o->defineDefaultProperty(QStringLiteral("gcCollect"), QV4::NetObject::method_gccollect);
    o->defineDefaultProperty(QStringLiteral("await"), QV4::NetObject::method_await);
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

    if (callData->argc != 2) {
        qWarning() << "Invalid number of parameters passed to Net.await(task, callback)";
        return;
    }

    ScopedValue task(scope, callData->args[0]);
    ScopedValue callback(scope, callData->args[1]);

    if(task->isNullOrUndefined()) {
        qWarning() << "No task for Net.await(task, callback)";
        return;
    }

    if(callback->isNullOrUndefined()) {
        qWarning() << "No callback for Net.await(task, callback)";
        return;
    }

    QJSValue taskJsValue(scope.engine, task->asReturnedValue());
    QJSValue callbackJsValue(scope.engine, callback->asReturnedValue());

    QObject* qObject = taskJsValue.toQObject();
    NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);

    if(!netValue) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return;
    }

    // Send the method to .NET, await the task, and call the callback.
    awaitTask(netValue->getNetReference(), QSharedPointer<NetJSValue>(new NetJSValue(callbackJsValue)));
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

    if(argc != 2) {
        qWarning() << "Invalid number of parameters passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    QV4::ScopedValue task(scope, argv[0]);
    QV4::ScopedValue callback(scope, argv[1]);

    if(task->isNullOrUndefined()) {
        qWarning() << "No task for Net.await(task, callback)";
        return Encode::undefined();
    }

    if(callback->isNullOrUndefined()) {
        qWarning() << "No callback for Net.await(task, callback)";
        return Encode::undefined();
    }

    QJSValue taskJsValue(scope.engine, task->asReturnedValue());
    QJSValue callbackJsValue(scope.engine, callback->asReturnedValue());

    if(!taskJsValue.isQObject()) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    QObject* qObject = taskJsValue.toQObject();
    NetValueInterface* netValue = qobject_cast<NetValueInterface*>(qObject);

    if(!netValue) {
        qWarning() << "Invalid task object passed to Net.await(task, callback)";
        return Encode::undefined();
    }

    // Send the method to .NET, await the task, and call the callback.
    awaitTask(netValue->getNetReference(), QSharedPointer<NetJSValue>(new NetJSValue(callbackJsValue)));

    return Encode::undefined();
}

#endif

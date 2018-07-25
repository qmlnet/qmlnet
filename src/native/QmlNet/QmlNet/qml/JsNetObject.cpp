#include <QmlNet/qml/JsNetObject.h>
#include <QmlNet/types/Callbacks.h>
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
    //QV4::ScopedValue task(scope, argv[0]);



    //QV4::ScopedValue callback(scope, argv[1]);


    //QJSValue taskJsValue(scope.engine, task->asReturnedValue());

    //QSharedPointer<NetVariant> = QSharedPointer<NetVariant>(taskVariant);
    //taskVariant.setVariant(QJSValue(scope.engine, task->asReturnedValue()).toVariant());

    //QJSValue retval(scope.engine, callback->asReturnedValue());



    return Encode::undefined();
}

#endif

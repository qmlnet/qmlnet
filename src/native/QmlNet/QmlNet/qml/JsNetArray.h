#ifndef JSNETARRAY_H
#define JSNETARRAY_H

#include <QmlNet/qml/NetValue.h>
#include <private/qv4functionobject_p.h>

namespace QV4 {

namespace Heap {

struct NetArray : Object {
    void init();
    void destroy();
    QV4::Value* object;
};

}

struct NetArray : Object
{
    V4_OBJECT2(NetArray, Object)
    V4_NEEDS_DESTROY
    Q_MANAGED_TYPE(ArrayObject)

    static ReturnedValue create(ExecutionEngine *engine, NetValue* netValue);

    #if QT_VERSION < QT_VERSION_CHECK(5, 11, 0)
    static void method_length(const BuiltinFunction *, Scope &, CallData *callData);
    static void method_push(const BuiltinFunction *, Scope &, CallData *callData);
    static void method_pop(const BuiltinFunction *, Scope &, CallData *callData);
    static void method_forEach(const BuiltinFunction *, Scope &, CallData *callData);
    #else
    static ReturnedValue method_length(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_push(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_pop(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_forEach(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    #endif

    static ReturnedValue getIndexed(const Managed *m, uint index, bool *hasProperty);
    static bool putIndexed(Managed *m, uint index, const Value &value);
};

}

#endif // JSNETARRAY_H

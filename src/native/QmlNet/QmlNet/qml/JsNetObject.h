#ifndef JSNETOBJECT_H
#define JSNETOBJECT_H

#include <private/qv4functionobject_p.h>

namespace QV4 {

namespace Heap {

struct NetObject : Object {
    void init();
};

}

struct NetObject : Object
{
    V4_OBJECT2(NetObject, Object)
    #if QT_VERSION < QT_VERSION_CHECK(5, 11, 0)
    static void method_gccollect(const BuiltinFunction *, Scope &scope, CallData *callData);
    static void method_await(const BuiltinFunction *, Scope &scope, CallData *callData);
    static void method_cancelTokenSource(const BuiltinFunction *, Scope &scope, CallData *callData);
    static void method_serialize(const BuiltinFunction *, Scope &scope, CallData *callData);
    static void method_toJsArray(const BuiltinFunction *, Scope &scope, CallData *callData);
    static void method_toListModel(const BuiltinFunction *, Scope &scope, CallData *callData);
    #else
    static ReturnedValue method_gccollect(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_await(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_cancelTokenSource(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_serialize(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_toJsArray(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue method_toListModel(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    #endif
};

}

#endif // JSNETOBJECT_H

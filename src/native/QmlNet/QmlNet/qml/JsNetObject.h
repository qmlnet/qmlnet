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
    #else
    static ReturnedValue method_gccollect(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    #endif
};

}

#endif // JSNETOBJECT_H

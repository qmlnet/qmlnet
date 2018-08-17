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

    static ReturnedValue method_length(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc);
    static ReturnedValue getIndexed(const Managed *m, uint index, bool *hasProperty);
    static bool putIndexed(Managed *m, uint index, const Value &value);
};

}

#endif // JSNETARRAY_H

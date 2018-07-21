#ifndef NET_QQMLAPPLICATIONENGINE_H
#define NET_QQMLAPPLICATIONENGINE_H

#include <QmlNet.h>
#include <QQmlApplicationEngine>
#include <QSharedPointer>
#include <private/qqmlglobal_p.h>
#include <private/qv4functionobject_p.h>
#include <private/qjsengine_p.h>
#include <private/qqmlengine_p.h>

struct QQmlApplicationEngineContainer {
    QSharedPointer<QQmlApplicationEngine> qmlEngine;
};

class QQmlEngine;

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

#endif // NET_QQMLAPPLICATIONENGINE_H

#include <QmlNet/qml/JsNetArray.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QVariant>
#include <private/qv4qobjectwrapper_p.h>

QT_BEGIN_NAMESPACE

using namespace QV4;

DEFINE_OBJECT_VTABLE(NetArray);

void Heap::NetArray::init()
{
    Object::init();
    QV4::Scope scope(internalClass->engine);
    QV4::ScopedObject o(scope, this);
    o->setArrayType(Heap::ArrayData::Custom);
    o->defineAccessorProperty(QStringLiteral("length"), QV4::NetArray::method_length, nullptr);
    o->defineDefaultProperty(QStringLiteral("push"), QV4::NetArray::method_push);
    o->defineDefaultProperty(QStringLiteral("pop"), QV4::NetArray::method_pop);
    o->defineDefaultProperty(QStringLiteral("forEach"), QV4::NetArray::method_forEach);
    object = scope.engine->memoryManager->m_persistentValues->allocate();
}

void Heap::NetArray::destroy()
{
    QV4::PersistentValueStorage::free(object);
    Object::destroy();
}

ReturnedValue NetArray::create(ExecutionEngine *engine, NetValue* netValue)
{
    Scope scope(engine);
    Scoped<NetArray> r(scope, engine->memoryManager->allocObject<NetArray>());
    *r->d()->object = QObjectWrapper::wrap(scope.engine, netValue);
    return r.asReturnedValue();
}

void NetArray::method_length(const BuiltinFunction *, Scope &scope, CallData *callData)
{
    ScopedObject instance(scope, callData->thisObject.toObject(scope.engine));
    NetArray* netArray = instance->as<NetArray>();
    Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeArrayFacade> arrayFacade = netValue->getNetReference()->getTypeInfo()->getArrayFacade();

    if(arrayFacade == nullptr) {
        THROW_GENERIC_ERROR("The wrapped object can't be treated as an array.");
    }

    scope.result = Encode(arrayFacade->getLength(netValue->getNetReference()));
}

void NetArray::method_push(const BuiltinFunction *, Scope &scope, CallData *callData)
{
    ScopedObject instance(scope, callData->thisObject.toObject(scope.engine));
    NetArray* netArray = instance->as<NetArray>();
    Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeArrayFacade> arrayFacade = netValue->getNetReference()->getTypeInfo()->getArrayFacade();

    if(arrayFacade == nullptr) {
        THROW_GENERIC_ERROR("The wrapped object can't be treated as an array.");
    }

    if(arrayFacade->isFixed()) {
        THROW_GENERIC_ERROR("Can't modify a fixed .NET list type.");
    }

    uint len = instance->getLength();
    if(len == 0) {
        RETURN_UNDEFINED();
    }

    for (int i = 0, ei = callData->argc; i < ei; ++i) {
        QV4::ScopedValue valueScope(scope, callData->args[i]);
        QJSValue valueJsValue(scope.engine, valueScope->asReturnedValue());
        arrayFacade->push(netValue->getNetReference(), NetVariant::fromQJSValue(valueJsValue));
    }

    scope.result = Encode(instance->getLength());
}

void NetArray::method_pop(const BuiltinFunction *, Scope &scope, CallData *callData)
{
    ScopedObject instance(scope, callData->thisObject.toObject(scope.engine));
    NetArray* netArray = instance->as<NetArray>();
    Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeArrayFacade> arrayFacade = netValue->getNetReference()->getTypeInfo()->getArrayFacade();

    if(arrayFacade == nullptr) {
        THROW_GENERIC_ERROR("The wrapped object can't be treated as an array.");
    }

    if(arrayFacade->isFixed()) {
        THROW_GENERIC_ERROR("Can't modify a fixed .NET list type.");
    }

    uint len = instance->getLength();
    if(len == 0) {
        RETURN_UNDEFINED();
    }

    QSharedPointer<NetVariant> result = arrayFacade->pop(netValue->getNetReference());
    if(result == nullptr) {
        scope.result = Encode::null();
    }
    QJSValue resultJsValue = result->toQJSValue(scope.engine->jsEngine());
    scope.result = scope.engine->fromVariant(resultJsValue.toVariant());
}

void NetArray::method_forEach(const BuiltinFunction *, Scope &scope, CallData *callData)
{
    ScopedObject instance(scope, callData->thisObject.toObject(scope.engine));
    if (!instance)
        RETURN_UNDEFINED();

    uint len = instance->getLength();

    ScopedFunctionObject callback(scope, callData->argument(0));
    if (!callback)
        THROW_TYPE_ERROR();

    ScopedCallData cData(scope, 3);
    cData->thisObject = callData->argument(1);
    cData->args[2] = instance;

    ScopedValue v(scope);
    for (uint k = 0; k < len; ++k) {
        bool exists;
        v = instance->getIndexed(k, &exists);
        if (!exists)
            continue;

        cData->args[0] = v;
        cData->args[1] = Primitive::fromDouble(k);
        callback->call(scope, cData);
    }
    RETURN_UNDEFINED();
}

ReturnedValue NetArray::getIndexed(const Managed *m, uint index, bool *hasProperty)
{
    const NetArray *netArray = static_cast<const NetArray*>(m);
    Scope scope(netArray->engine());
    QV4::Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeArrayFacade> arrayFacade = netValue->getNetReference()->getTypeInfo()->getArrayFacade();

    if(hasProperty)
        *hasProperty = true;

    if(arrayFacade == nullptr) {
        return scope.engine->throwError(QString::fromUtf8("The wrapped object can't be treated as an array."));
    }

    QSharedPointer<NetVariant> result = arrayFacade->getIndexed(netValue->getNetReference(), index);
    QJSValue resultJsValue = result->toQJSValue(scope.engine->jsEngine());
    return scope.engine->fromVariant(resultJsValue.toVariant());
}

bool NetArray::putIndexed(Managed *m, uint index, const Value &value)
{
    const NetArray *netArray = static_cast<const NetArray*>(m);
    Scope scope(netArray->engine());
    QV4::Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeArrayFacade> arrayFacade = netValue->getNetReference()->getTypeInfo()->getArrayFacade();

    if(arrayFacade == nullptr) {
        return scope.engine->throwError(QString::fromUtf8("The wrapped object can't be treated as an array."));
    }

    QV4::ScopedValue valueScope(scope, value);
    QJSValue valueJsValue(scope.engine, valueScope->asReturnedValue());
    arrayFacade->setIndexed(netValue->getNetReference(), index, NetVariant::fromQJSValue(valueJsValue));

    return true;
}

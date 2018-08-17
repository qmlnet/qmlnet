#include <QmlNet/qml/JsNetArray.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/Callbacks.h>
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

ReturnedValue NetArray::method_length(const FunctionObject *b, const Value *thisObject, const Value *argv, int argc)
{
    Scope scope(b);
    return Encode(scope.engine->newNumberObject(1));
}

ReturnedValue NetArray::getIndexed(const Managed *m, uint index, bool *hasProperty)
{
    const NetArray *netArray = static_cast<const NetArray*>(m);
    Scope scope(netArray->engine());
    if(hasProperty)
        *hasProperty = true;

    // Find the "Get" method for this type.

    QV4::Scoped<QV4::QObjectWrapper> wrapper(scope, netArray->d()->object);
    if (!wrapper) {
        THROW_GENERIC_ERROR("No reference to the wrapped QObject exists.");
    }

    NetValue* netValue = reinterpret_cast<NetValue*>(wrapper->d()->object());
    QSharedPointer<NetTypeInfo> typeInfo = netValue->getNetReference()->getTypeInfo();

    QSharedPointer<NetMethodInfo> getMethod;
    for(int x = 0; x < typeInfo->getMethodCount(); x++) {
        QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getMethodInfo(x);
        if(methodInfo->getMethodName().compare("Get") == 0) {
            getMethod = methodInfo;
            break;
        }
    }

    if(getMethod == nullptr) {
        THROW_GENERIC_ERROR("Couldn't find the Get method for the .NET array");
    }

    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    invokeNetMethod(getMethod, netValue->getNetReference(), parameters, result);

    QJSValue resultJsValue = result->toQJSValue(scope.engine->jsEngine());

    if(resultJsValue.isNull() || resultJsValue.isUndefined()) {
        THROW_GENERIC_ERROR("Couldn't find the Get method for the .NET array");
    }

    return scope.engine->fromVariant(resultJsValue.toVariant());
}

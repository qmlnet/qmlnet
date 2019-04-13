#include <QmlNet/qml/JsNetObject.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeManager.h>
#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNet/qml/NetListModel.h>
#include <QmlNet/qml/QQmlApplicationEngine.h>
#include <QQmlEngine>
#include <QDebug>

JsNetObject::JsNetObject() = default;

QString JsNetObject::serialize(const QJSValue& value)
{
    if(value.isNull() || value.isUndefined()) {
        qWarning() << "Net.serialize(): Instance parameter must not be null or undefined";
        return QString();
    }

    QSharedPointer<NetVariant> netVaraint = NetVariant::fromQJSValue(value);
    if(netVaraint->getVariantType() != NetVariantTypeEnum_Object) {
        qWarning() << "Net.serialize(): Parameter is not a .NET object";
        return QString();
    }

    QSharedPointer<NetVariant> result(new NetVariant());
    bool serializationResult = QmlNet::serializeNetToString(netVaraint->getNetReference(), result);
    if(!serializationResult) {
        qWarning() << "Net.serialize(): Could not serialize object.";
        return QString();
    }

    return result->getString();
}

QVariant JsNetObject::cancelTokenSource()
{
    QSharedPointer<NetTypeInfo> typeInfo = NetTypeManager::getTypeInfo("System.Threading.CancellationTokenSource");
    if(typeInfo == nullptr) {
        qWarning() << "Couldn't get cancellation token type for platform, please file a bug";
        return QVariant();
    }
    QSharedPointer<NetReference> netReference = QmlNet::instantiateType(typeInfo);
    if(netReference == nullptr) {
        qWarning() << "Couldn't create cancellation token for platform, please file a bug";
        return QVariant();
    }
    QSharedPointer<NetVariant> netVariant(new NetVariant());
    netVariant->setNetReference(netReference);
    return netVariant->toQVariant();
}

void JsNetObject::gcCollect(int maxGeneration)
{
    QmlNet::gcCollect(maxGeneration);
}

QVariant JsNetObject::toListModel(const QJSValue& value)
{
    if(value.isNull() || value.isUndefined()) {
        qWarning() << "Net.toListModel(): Instance parameter must not be null or undefined";
        return QVariant();
    }

    QSharedPointer<NetVariant> netVaraint = NetVariant::fromQJSValue(value);
    if(netVaraint->getVariantType() != NetVariantTypeEnum_Object) {
        qWarning() << "Net.toListModel(): Parameter is not a .NET object";
        return QVariant();
    }

    NetListModel* listModel = NetListModel::fromReference(netVaraint->getNetReference());
    if(listModel == nullptr) {
        qWarning() << "Net.toListModel(): Parameter is not a type that can be wrapped by a list model.";
        return QVariant();
    }

    QQmlEngine::setObjectOwnership(listModel, QQmlEngine::JavaScriptOwnership);
    return QVariant::fromValue(listModel);
}

Q_INVOKABLE QVariant JsNetObject::listForEach(const QJSValue& value, QJSValue callback)
{
    if(value.isNull() || value.isUndefined()) {
        qWarning() << "Net.listForEach(): Instance parameter must not be null or undefined";
        return QVariant();
    }

    if(callback.isNull() || callback.isUndefined()) {
        qWarning() << "Net.listForEach(): Callback must not be null or undefined";
        return QVariant();
    }

    if(!callback.isCallable()) {
        qWarning() << "Net.listForEach(): Callback is not a function";
        return QVariant();
    }

    QSharedPointer<NetVariant> netVaraint = NetVariant::fromQJSValue(value);
    if(netVaraint->getVariantType() != NetVariantTypeEnum_Object) {
        qWarning() << "Net.listForEach(): Parameter is not a .NET object";
        return QVariant();
    }

    QSharedPointer<NetReference> netReference = netVaraint->getNetReference();

    QSharedPointer<NetTypeArrayFacade> facade = netReference->getTypeInfo()->getArrayFacade();
    if(facade == nullptr) {
        qWarning() << "Net.listForEach(): Parameter is not a type that be enumerated.";
        return QVariant();
    }

    uint count = facade->getLength(netReference);

    for(uint x = 0; x < count; x++) {
        QSharedPointer<NetVariant> item = facade->getIndexed(netReference, x);
        QJSValueList args;
        args.push_back(item->toQJSValue());
        args.push_back(sharedQmlEngine()->toScriptValue<QVariant>(QVariant::fromValue(x)));
        callback.call(args);
    }

    return QVariant::fromValue(count);
}

void JsNetObject::toJsArray()
{
    qWarning() << "Net.toJsArray(): Not supported anymore. Use Net.toListModel().";
}

void JsNetObject::await(const QJSValue& task, const QJSValue& successCallback, const QJSValue& failureCallback)
{
    if(task.isNull() || task.isUndefined()) {
        qWarning() << "Net.await(): No task object provided.";
        return;
    }

    if(successCallback.isNull() || successCallback.isUndefined()) {
        qWarning() << "Net.await(): Not success callback given";
        return;
    }

    if(!successCallback.isCallable()) {
        qWarning() << "Net.await(): Success callback invalid type.";
        return;
    }

    if(!failureCallback.isNull() && !failureCallback.isUndefined()) {
        if(!failureCallback.isCallable()) {
            qWarning() << "Net.await(): Failure callback invalid type.";
            return;
        }
    }

    QSharedPointer<NetVariant> taskVariant = NetVariant::fromQJSValue(task);
    if(taskVariant->getVariantType() != NetVariantTypeEnum_Object) {
        qWarning() << "Net.await(): Task is invalid type.";
        return;
    }

    QSharedPointer<NetVariant> successCallbackVariant = NetVariant::fromQJSValue(successCallback);
    QSharedPointer<NetVariant> failureCallbackVariant = NetVariant::fromQJSValue(failureCallback);
    QmlNet::awaitTask(taskVariant->getNetReference(),
                      successCallbackVariant->getJsValue(),
                      failureCallbackVariant != nullptr ? failureCallbackVariant->getJsValue() : nullptr);
}

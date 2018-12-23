#include <QmlNet/qml/JsNetObject.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeManager.h>
#include <QmlNet/qml/NetListModel.h>
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

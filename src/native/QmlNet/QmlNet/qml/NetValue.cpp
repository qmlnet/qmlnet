#include <QQmlEngine>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetValueMetaObject.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QmlNet/types/Callbacks.h>
#include <QDebug>
#include <utility>

NetValue::~NetValue()
{
    auto objectId = instance->getObjectId();
    auto hit = objectIdNetValuesMap.find(objectId);
    if(hit != objectIdNetValuesMap.end()) {
        NetValueCollection* collection = hit.value();
        collection->netValues.removeOne(this);
        if(collection->netValues.length() == 0) {
            objectIdNetValuesMap.remove(objectId);
            delete collection;
        }
    }

    if(this->instance->getTypeInfo()->hasObjectDestroyed()){
        QmlNet::callObjectDestroyed(instance);
    }

#ifdef QMLNET_TRACE
    qDebug("netvalue: destroyed: for: %s", qPrintable(instance->displayName()));
#endif
    instance = nullptr;
}


QSharedPointer<NetReference> NetValue::getNetReference()
{
    return instance;
}

bool NetValue::activateSignal(const QString& signalName, const QSharedPointer<NetVariantList>& arguments)
{
    int signalMethodIndex = -1;
    for(int x = 0; valueMeta->methodCount(); x++) {
        QByteArray methodName = valueMeta->method(x).name();
        if(signalName.compare(methodName) == 0) {
            signalMethodIndex = x;
            break;
        }
    }

    // If signal not found, dump the registered signals for debugging.
    if(signalMethodIndex < 0) {
        qDebug() << "Signal not found:" << signalName;
        qDebug() << "Current signals:";
        for (int i = 0; i < metaObject()->methodCount(); i++) {
            QMetaMethod method = metaObject()->method(i);
            if (method.methodType() == QMetaMethod::Signal) {
                qDebug().nospace() << "\t" << method.methodSignature();
            }
        }
        return false;
    }

    // Build the types needed to activate the signal
    QList<QSharedPointer<QVariant>> variantArgs;
    std::vector<void*> voidArgs;
    voidArgs.push_back(nullptr); // For the return type, which is nothing for signals.
    if(arguments != nullptr) {
        for(int x = 0 ; x < arguments->count(); x++) {
            QSharedPointer<QVariant> variant(new QVariant(arguments->get(x)->toQVariant()));
            variantArgs.append(variant);
            voidArgs.push_back(static_cast<void*>(variant.data()));
        }
    }
    void** argsPointer = nullptr;
    if(!voidArgs.empty()) {
        argsPointer = &voidArgs[0];
    }

    // Activate the signal!
    NetValueMetaObject::activate(this, signalMethodIndex, argsPointer);

    return true;
}

NetValue* NetValue::forInstance(const QSharedPointer<NetReference>& instance)
{
    NetValue* result = new NetValue(instance, nullptr);
    QQmlEngine::setObjectOwnership(result, QQmlEngine::JavaScriptOwnership);
    return result;
}

QList<NetValue*> NetValue::getAllLiveInstances(const QSharedPointer<NetReference>& instance)
{
    auto objectId = instance->getObjectId();
    NetValueCollection* collection = nullptr;
    auto hit = objectIdNetValuesMap.find(objectId);
    if(hit != objectIdNetValuesMap.end()) {
        collection = hit.value();
        return collection->netValues;
    }
    return QList<NetValue*>();
}

void NetValue::classBegin()
{

}

void NetValue::componentComplete()
{
    if(this->instance->getTypeInfo()->hasComponentCompleted()){
        QmlNet::callComponentCompleted(instance);
    }
}

NetValue::NetValue(const QSharedPointer<NetReference>& instance, QObject *parent)
    : instance(instance)
{
#ifdef QMLNET_TRACE
    qDebug("netvalue: created: for: %s", qPrintable(instance->displayName()));
#endif

    valueMeta = new NetValueMetaObject(this, instance);
    setParent(parent);

    auto objectId = instance->getObjectId();
    NetValueCollection* collection = nullptr;
    auto hit = objectIdNetValuesMap.find(objectId);
    if(hit != objectIdNetValuesMap.end()) {
        collection = hit.value();
    } else {
        collection = new NetValueCollection();
        objectIdNetValuesMap.insert(objectId, collection);
    }
    collection->netValues.append(this);

    // Auto wire up all of our signal handlers that will invoke .NET delegates.
    for(int index = 0; index <= instance->getTypeInfo()->getSignalCount() - 1; index++)
    {
        QSharedPointer<NetSignalInfo> signalInfo = instance->getTypeInfo()->getSignal(index);

        QString signalSig = signalInfo->getSignature();
        QString slotSig = signalInfo->getSlotSignature();

        int signalIndex = valueMeta->indexOfSignal(signalSig.toLatin1().data());
        int slotIndex = valueMeta->indexOfSlot(slotSig.toLatin1().data());

        QMetaMethod signalMethod = valueMeta->method(signalIndex);
        QMetaMethod slotMethod = valueMeta->method(slotIndex);

        QObject::connect(this, signalMethod,
                         this, slotMethod);
    };
}

QMap<uint64_t, NetValue::NetValueCollection*> NetValue::objectIdNetValuesMap = QMap<uint64_t, NetValue::NetValueCollection*>();

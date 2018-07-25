#include <QQmlEngine>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetValueMetaObject.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QDebug>

NetValue::~NetValue()
{
    auto hit = objectIdNetValuesMap.find(instance->getObjectId());
    if(hit != objectIdNetValuesMap.end())
    {
        objectIdNetValuesMap.erase(hit);
    }
    instance = nullptr;
}


QSharedPointer<NetReference> NetValue::getNetReference()
{
    return instance;
}

bool NetValue::activateSignal(QString signalName, QSharedPointer<NetVariantList> arguments)
{
    // Build the signature so we can look it up.
    // Perf?
    QString signature = signalName;
    signature.append("(");
    if(arguments != NULL) {
        for(int argumentIndex = 0; argumentIndex <= arguments->count() - 1; argumentIndex++)
        {
            if(argumentIndex > 0) {
                signature.append(",");
            }
            signature.append("QVariant");
        }
    }
    signature.append(")");
    QByteArray normalizedSignalSignature = QMetaObject::normalizedSignature(signature.toLatin1().data());
    int signalMethodIndex = valueMeta->indexOfMethod(normalizedSignalSignature);

    // If signal not found, dump the registered signals for debugging.
    if(signalMethodIndex < 0) {
        qDebug("Signal not found: %s", qPrintable(normalizedSignalSignature));
        qDebug("Current signals:");
        for (int i = 0; i < metaObject()->methodCount(); i++) {
            QMetaMethod method = metaObject()->method(i);
            if (method.methodType() == QMetaMethod::Signal) {
                qDebug("\t%s", qPrintable(method.methodSignature()));
            }
        }
        return false;
    }

    // Build the types needed to activate the signal
    QList<QSharedPointer<QVariant>> variantArgs;
    std::vector<void*> voidArgs;
    voidArgs.push_back(NULL); // For the return type, which is nothing for signals.
    if(arguments != NULL) {
        for(int x = 0 ; x < arguments->count(); x++) {
            QSharedPointer<QVariant> variant = QSharedPointer<QVariant>(new QVariant(arguments->get(x)->asQVariant()));
            variantArgs.append(variant);
            voidArgs.push_back((void *)variant.data());
        }
    }
    void** argsPointer = nullptr;
    if(voidArgs.size() > 0) {
        argsPointer = &voidArgs[0];
    }

    // Activate the signal!
    valueMeta->activate(this, signalMethodIndex, argsPointer);

    return true;
}

NetValue* NetValue::forInstance(QSharedPointer<NetReference> instance, bool autoCreate)
{
    auto objectId = instance->getObjectId();
    if(objectIdNetValuesMap.find(objectId) != objectIdNetValuesMap.end())
    {
        return objectIdNetValuesMap.at(objectId);
    }
    if(!autoCreate)
    {
        return nullptr;
    }
    auto result = new NetValue(instance, nullptr);
    QQmlEngine::setObjectOwnership(result, QQmlEngine::JavaScriptOwnership);
    return result;
}

NetValue::NetValue(QSharedPointer<NetReference> instance, QObject *parent)
    : instance(instance)
{
    valueMeta = new NetValueMetaObject(this, instance);
    setParent(parent);
    if(instance->getTypeInfo()->getSignalCount() > 0) {
        // Auto wire up all of our signal handlers that will invoke .NET delegates.
        for(uint index = 0; index <= instance->getTypeInfo()->getSignalCount() - 1; index++)
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
        }
    }
    objectIdNetValuesMap[instance->getObjectId()] = this;
}

std::map<uint64_t, NetValue*> NetValue::objectIdNetValuesMap = std::map<uint64_t, NetValue*>();

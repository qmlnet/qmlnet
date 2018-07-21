#include <QQmlEngine>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetValueMetaObject.h>
#include <QDebug>

NetValue::~NetValue()
{
    auto hit = netValues.find(instance.data());
    if(hit != netValues.end())
    {
        netValues.erase(hit);
    }
    qDebug("NetValue deleted: %s", qPrintable(instance->getTypeInfo()->getClassName()));
    if(instance != nullptr) {
        instance->release();
    }
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
    if(netValues.find(instance.data()) != netValues.end())
    {
        return netValues.at(instance.data());
    }
    if(!autoCreate)
    {
        return NULL;
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
    netValues[instance.data()] = this;
    qDebug("NetValue created: %s", qPrintable(instance->getTypeInfo()->getClassName()));
}

std::map<NetReference*, NetValue*> NetValue::netValues = std::map<NetReference*, NetValue*>();

#include "net_qml_value.h"
#include "net_type_info_manager.h"
#include "net_variant.h"

#include <QMetaMethod>

NetValue::NetValue(NetInstance *instance, QObject *parent)
    : instance(instance)
{

    valueMeta = new GoValueMetaObject(this, instance);
    setParent(parent);

    instance->GetTypeInfo()->RegisterNetInstance(instance->GetGCHandle(), this);
}

NetValue::~NetValue()
{
    instance->GetTypeInfo()->UnregisterNetInstance(this);
    delete instance;
    instance = NULL;
}


NetInstance* NetValue::GetNetInstance()
{
    return instance;
}

void NetValue::ActivateSignal(std::string signalName, std::vector<NetVariant*> args)
{
    auto normalizedSignalSignature = QMetaObject::normalizedSignature(signalName.c_str());
    auto signalMethodIndex = metaObject()->indexOfMethod(normalizedSignalSignature);
    if(signalMethodIndex < 0)
    {
        qDebug() << "Signal not found: " << normalizedSignalSignature << "Signal list:";

        for (int i = 0; i < metaObject()->methodCount(); i++) {
            QMetaMethod method = metaObject()->method(i);
            if (method.methodType() == QMetaMethod::Signal) {
                qDebug() << "Signal: " << method.name() << ", Sig: " << method.methodSignature();
            }
        }
        throw std::invalid_argument("Signal not found! Unable to send signal '" + signalName + "' of Type '" + instance->GetTypeInfo()->GetFullTypeName() + "'");
    }
    //TODO: Not sure about this argument handling :(
    QList<QVariant*> qVariantArgs;
    std::vector<void *> voidArgs;
    for(const auto& arg : args)
    {
        QVariant* entry = new QVariant(arg->AsQVariant());
        qVariantArgs.append(entry);
        voidArgs.push_back((void *)entry);
    }
    void** argsPointer = nullptr;
    if(voidArgs.size() > 0)
    {
        argsPointer = &voidArgs[0];
    }
    valueMeta->activate(this, signalMethodIndex, argsPointer);
    for(const auto& qVariantArg : qVariantArgs)
    {
        delete qVariantArg;
    }
}

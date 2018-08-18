#include <QmlNet/qml/NetValueMetaObject.h>
#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QmlNet/types/Callbacks.h>
#include <QQmlEngine>
#include <QDebug>
#include <private/qmetaobjectbuilder_p.h>

QMetaObject *metaObjectFor(QSharedPointer<NetTypeInfo> typeInfo)
{
    if (typeInfo->metaObject) {
        return reinterpret_cast<QMetaObject *>(typeInfo->metaObject);
    }

    typeInfo->ensureLoaded();

    QMetaObjectBuilder mob;
    mob.setSuperClass(&QObject::staticMetaObject);
    mob.setClassName(typeInfo->getClassName().toLatin1());
    mob.setFlags(QMetaObjectBuilder::DynamicMetaObject);

    // register all the signals for the type
    for(int index = 0; index <= typeInfo->getSignalCount() - 1; index++)
    {
        QSharedPointer<NetSignalInfo> signalInfo = typeInfo->getSignal(index);
        QString signature = signalInfo->getSignature();
        mob.addSignal(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()));
    }

    // NOTE: It is important to register properties after the signals (before methods)
    // because of the assumptions we make about getting the "notifySignal" by index.
    for(int index = 0; index <= typeInfo->getPropertyCount() - 1; index++)
    {
        QSharedPointer<NetPropertyInfo> propertyInfo = typeInfo->getProperty(index);
        QSharedPointer<NetTypeInfo> propertyType = propertyInfo->getReturnType();
        QString propertyName = propertyInfo->getPropertyName();
        if(propertyName.at(0).isUpper()) {
            propertyName.replace(0,1, propertyName.at(0).toLower());
        }
        QMetaPropertyBuilder propb = mob.addProperty(propertyName.toLatin1(),
            NetMetaValueQmlType(propertyType->getPrefVariantType()),
            index);
        QSharedPointer<NetSignalInfo> notifySignal = propertyInfo->getNotifySignal();
        if(notifySignal != nullptr) {
            // The signal was previously registered, find the index.
            for(int signalIndex = 0; signalIndex <= typeInfo->getSignalCount() - 1; signalIndex++)
            {
                if(typeInfo->getSignal(signalIndex) == notifySignal) {
                    QMetaMethodBuilder notifySignalBuilder = mob.method(signalIndex);
                    propb.setNotifySignal(notifySignalBuilder);
                    break;
                }
            }
        }
        propb.setWritable(propertyInfo->canWrite());
        propb.setReadable(propertyInfo->canRead());
    }

    for(int index = 0; index <= typeInfo->getLocalMethodCount() - 1; index++)
    {
        QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getLocalMethodInfo(index);
        QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
        QString signature = methodInfo->getSignature();
        if(returnType != nullptr) {
            mob.addMethod(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()),
                NetMetaValueQmlType(returnType->getPrefVariantType()));
        } else {
            mob.addMethod(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()));
        }
    }

    // For every signal that was added, add an associated slot
    // so that we can auto-hook the slot to each signal so that
    // we can raise .NET-attached delegates.
    for(int index = 0; index <= typeInfo->getSignalCount() - 1; index++)
    {
        QSharedPointer<NetSignalInfo> signalInfo = typeInfo->getSignal(index);
        QString signature = signalInfo->getSlotSignature();
        mob.addSlot(signature.toLatin1().data());
    }

    QMetaObject *mo = mob.toMetaObject();
    typeInfo->metaObject = mo;
    return mo;
}

NetValueMetaObject::NetValueMetaObject(QObject *value,
                                       QSharedPointer<NetReference> instance) :
    value(value),
    instance(instance)
{
    *static_cast<QMetaObject *>(this) = *metaObjectFor(instance->getTypeInfo());

    QObjectPrivate *objPriv = QObjectPrivate::get(value);
    objPriv->metaObject = this;
}

int NetValueMetaObject::metaCall(QMetaObject::Call c, int idx, void **a)
{
    switch(c) {
    case ReadProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        QSharedPointer<NetPropertyInfo> propertyInfo = instance->getTypeInfo()->getProperty(idx - offset);
        QSharedPointer<NetTypeInfo> propertyType = propertyInfo->getReturnType();

        QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
        readProperty(propertyInfo, instance, result);

        NetMetaValuePack(propertyType->getPrefVariantType(), result, a[0]);
    }
        break;
    case WriteProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        QSharedPointer<NetPropertyInfo> propertyInfo = instance->getTypeInfo()->getProperty(idx - offset);
        QSharedPointer<NetTypeInfo> propertyType = propertyInfo->getReturnType();

        QSharedPointer<NetVariant> newValue = QSharedPointer<NetVariant>(new NetVariant());
        NetMetaValueUnpack(propertyType->getPrefVariantType(), newValue, a[0]);

        writeProperty(propertyInfo, instance, newValue);
    }
        break;
    case  InvokeMetaMethod:
    {
        int offset = methodOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        idx -= offset;
        if(idx < instance->getTypeInfo()->getSignalCount()) {
            // This is a signal call, activate it!
            activate(value, idx + offset, a);
            return -1;
        }

        idx -= instance->getTypeInfo()->getSignalCount();

        if(idx < instance->getTypeInfo()->getLocalMethodCount()) {
            // This is a method call!

            QSharedPointer<NetMethodInfo> methodInfo = instance->getTypeInfo()->getLocalMethodInfo(idx);
            QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());

            for(int index = 0; index <= methodInfo->getParameterCount() - 1; index++)
            {
                QSharedPointer<NetMethodInfoArguement> parameter = methodInfo->getParameter(index);
                QSharedPointer<NetTypeInfo> parameterType = parameter->getType();
                QSharedPointer<NetVariant> netVariant = QSharedPointer<NetVariant>(new NetVariant());
                NetMetaValueUnpack(parameterType->getPrefVariantType(), netVariant, a[index + 1]);
                parameters->add(netVariant);
            }

            QSharedPointer<NetVariant> result;
            QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
            if(returnType != nullptr) {
                result = QSharedPointer<NetVariant>(new NetVariant());
            }

            invokeNetMethod(methodInfo, instance, parameters, result);

            if(result != nullptr) {
                NetMetaValuePack(returnType->getPrefVariantType(), result, a[0]);
            }

            return -1;
        }

        idx -= instance->getTypeInfo()->getLocalMethodCount();

        {
            // This is a slot invocation, likely the built-in handlers that are used
            // to trigger NET delegates for any signals.
            QSharedPointer<NetSignalInfo> signalInfo = instance->getTypeInfo()->getSignal(idx);
            QSharedPointer<NetVariantList> parameters;

            if(signalInfo->getParameterCount() > 0) {
                parameters = QSharedPointer<NetVariantList>(new NetVariantList());
                for(int index = 0; index <= signalInfo->getParameterCount() - 1; index++)
                {
                    NetVariantTypeEnum parameterType = signalInfo->getParameter(index);

                    QSharedPointer<NetVariant> netVariant = QSharedPointer<NetVariant>(new NetVariant());
                    NetMetaValueUnpack(parameterType, netVariant, a[index + 1]);
                    parameters->add(netVariant);
                }
            }

            raiseNetSignals(instance, signalInfo->getName(), parameters);
        }
    }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

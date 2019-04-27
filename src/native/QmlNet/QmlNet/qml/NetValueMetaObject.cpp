#include <QmlNet/qml/NetValueMetaObject.h>
#include <QmlNet/qml/NetValueMetaObjectPacker.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeManager.h>
#include <QQmlEngine>
#include <QDebug>
#include <private/qmetaobjectbuilder_p.h>

QMetaObject *metaObjectFor(const QSharedPointer<NetTypeInfo>& typeInfo)
{
    if (typeInfo->metaObject) {
        return typeInfo->metaObject;
    }

    typeInfo->ensureLoaded();

    QMetaObjectBuilder mob;
    mob.setClassName(typeInfo->getClassName().toLatin1());
    mob.setFlags(QMetaObjectBuilder::DynamicMetaObject);

    QString baseType = typeInfo->getBaseType();
    if(baseType.isNull() || baseType.isEmpty()) {
        mob.setSuperClass(&QObject::staticMetaObject);
    } else {
        auto baseTypeInfo = NetTypeManager::getTypeInfo(baseType);
        mob.setSuperClass(metaObjectFor(baseTypeInfo));
    }

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
            "QVariant",
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
            mob.addMethod(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()), "QVariant");
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
                                       const QSharedPointer<NetReference>& instance) :
    value(value),
    instance(instance)
{
    *static_cast<QMetaObject *>(this) = *metaObjectFor(instance->getTypeInfo());

    QObjectPrivate *objPriv = QObjectPrivate::get(value);
    objPriv->metaObject = this;
}

int NetValueMetaObject::metaCall(QMetaObject::Call c, int idx, void **a)
{
#ifdef QMLNET_TRACE
    switch(c) {
    case ReadProperty:
    {
        auto prop = property(idx);
        qDebug() << this->className() << ": reading property: " << idx << ": " << prop.name();
    }
        break;
    case WriteProperty:
    {
        auto prop = property(idx);
        qDebug() << this->className() << ": writing property: " << idx << ": " << prop.name();
    }
        break;
    case  InvokeMetaMethod:
    {
        auto meth = method(idx);
        qDebug() << this->className() << ": invoking method: " << idx << ": " << meth.name();
    }
        break;
    default:
        break; // Unhandled.
    }
#endif
    return metaCallRecursive(c, idx, idx, a, instance->getTypeInfo());
}

int NetValueMetaObject::metaCallRecursive(QMetaObject::Call c, int originalIdx, int idx, void **a, QSharedPointer<NetTypeInfo> typeInfo)
{
    switch(c) {
    case ReadProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            auto baseType = NetTypeManager::getBaseType(typeInfo);
            if(baseType != nullptr) {
                return metaCallRecursive(c, originalIdx, idx + baseType->getPropertyCount(), a, baseType);
            }
            return value->qt_metacall(c, idx, a);
        }

        QSharedPointer<NetPropertyInfo> propertyInfo = typeInfo->getProperty(idx - offset);
        QSharedPointer<NetTypeInfo> propertyType = propertyInfo->getReturnType();

        QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());

#ifdef QMLNET_TRACE
        qDebug("begin: read: %s.%s", qPrintable(instance->displayName()),
               qPrintable(propertyInfo->getPropertyName()));
#endif

        QmlNet::readProperty(propertyInfo, instance, nullptr, result);

#ifdef QMLNET_TRACE
        qDebug("end:   read: %s.%s value: %s", qPrintable(instance->displayName()),
               qPrintable(propertyInfo->getPropertyName()),
               qPrintable(result->getDisplayValue()));
#endif

        NetMetaValuePack(result, a[0]);
    }
        break;
    case WriteProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            auto baseType = NetTypeManager::getBaseType(typeInfo);
            if(baseType != nullptr) {
                return metaCallRecursive(c, originalIdx, idx + baseType->getPropertyCount(), a, baseType);
            }
            return value->qt_metacall(c, idx, a);
        }

        QSharedPointer<NetPropertyInfo> propertyInfo = typeInfo->getProperty(idx - offset);
        QSharedPointer<NetTypeInfo> propertyType = propertyInfo->getReturnType();

        QSharedPointer<NetVariant> newValue = QSharedPointer<NetVariant>(new NetVariant());
        NetMetaValueUnpack(newValue, a[0]);

        QmlNet::writeProperty(propertyInfo, instance, nullptr, newValue);
    }
        break;
    case  InvokeMetaMethod:
    {
        int offset = methodOffset();
        if (idx < offset) {
            auto baseType = NetTypeManager::getBaseType(typeInfo);
            if(baseType != nullptr) {
                return metaCallRecursive(c, originalIdx, idx + ((baseType->getSignalCount() * 2) + baseType->getLocalMethodCount()), a, baseType);
            }
            return value->qt_metacall(c, idx, a);
        }

        idx -= offset;
        if(idx < typeInfo->getSignalCount()) {
            // This is a signal call, activate it!
            activate(value, originalIdx, a);
            return -1;
        }

        idx -= typeInfo->getSignalCount();

        if(idx < typeInfo->getLocalMethodCount()) {
            // This is a method call!

            QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getLocalMethodInfo(idx);
            QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());

            for(int index = 0; index <= methodInfo->getParameterCount() - 1; index++)
            {
                QSharedPointer<NetMethodInfoArguement> parameter = methodInfo->getParameter(index);
                QSharedPointer<NetTypeInfo> parameterType = parameter->getType();
                QSharedPointer<NetVariant> netVariant = QSharedPointer<NetVariant>(new NetVariant());
                NetMetaValueUnpack(netVariant, a[index + 1]);
                parameters->add(netVariant);
            }

            QSharedPointer<NetVariant> result;
            QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
            if(returnType != nullptr) {
                result = QSharedPointer<NetVariant>(new NetVariant());
            }

            QmlNet::invokeNetMethod(methodInfo, instance, parameters, result);

#ifdef QMLNET_TRACE
            if(result != nullptr) {
                qDebug("end:   method: %s.%s(%s) result: %s", qPrintable(instance->displayName()),
                       qPrintable(methodInfo->getMethodName()),
                       qPrintable(parameters->debugDisplay()),
                       qPrintable(result->getDisplayValue()));
            } else  {
                qDebug("end:   method: %s.%s(%s)", qPrintable(instance->displayName()),
                       qPrintable(methodInfo->getMethodName()),
                       qPrintable(parameters->debugDisplay()));
            }
#endif

            if(result != nullptr) {
                NetMetaValuePack(result, a[0]);
            }

            return -1;
        }

        idx -= typeInfo->getLocalMethodCount();

        {
            // This is a slot invocation, likely the built-in handlers that are used
            // to trigger NET delegates for any signals.
            QSharedPointer<NetSignalInfo> signalInfo = typeInfo->getSignal(idx);
            QSharedPointer<NetVariantList> parameters;

            if(signalInfo->getParameterCount() > 0) {
                parameters = QSharedPointer<NetVariantList>(new NetVariantList());
                for(int index = 0; index <= signalInfo->getParameterCount() - 1; index++)
                {
                    QSharedPointer<NetVariant> netVariant = QSharedPointer<NetVariant>(new NetVariant());
                    NetMetaValueUnpack(netVariant, a[index + 1]);
                    parameters->add(netVariant);
                }
            }

            QmlNet::raiseNetSignals(instance, signalInfo->getName(), parameters);
        }
    }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

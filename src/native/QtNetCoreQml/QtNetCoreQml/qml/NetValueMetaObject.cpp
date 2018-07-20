#include <QtNetCoreQml/qml/NetValueMetaObject.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/types/NetMethodInfo.h>
#include <QtNetCoreQml/types/NetPropertyInfo.h>
#include <QtNetCoreQml/types/NetSignalInfo.h>
#include <QtNetCoreQml/types/Callbacks.h>
#include <QQmlEngine>
#include <QDebug>
#include <private/qmetaobjectbuilder_p.h>

void metaPackValue(QSharedPointer<NetVariant> source, QVariant* destination) {
    switch(source->getVariantType()) {
    case NetVariantTypeEnum_Invalid:
        destination->clear();
        break;
    case NetVariantTypeEnum_Bool:
        destination->setValue(source->getBool());
        break;
    case NetVariantTypeEnum_Char:
        destination->setValue(source->getChar());
        break;
    case NetVariantTypeEnum_Int:
        destination->setValue(source->getInt());
        break;
    case NetVariantTypeEnum_UInt:
        destination->setValue(source->getUInt());
        break;
    case NetVariantTypeEnum_Double:
        destination->setValue(source->getDouble());
        break;
    case NetVariantTypeEnum_String:
        destination->setValue(source->getString());
        break;
    case NetVariantTypeEnum_DateTime:
        destination->setValue(source->getDateTime());
        break;
    case NetVariantTypeEnum_Object:
    {
        QSharedPointer<NetInstance> newInstance = source->getNetInstance();
        NetValue* netValue = NetValue::forInstance(newInstance);
        destination->setValue(netValue);
        break;
    }
    default:
        qDebug() << "Unsupported variant type: " << source->getVariantType();
        break;
    }
}

void metaUnpackValue(QSharedPointer<NetVariant> destination, QVariant* source, NetVariantTypeEnum prefType) {
    bool ok = false;

    if(source->isNull()) {
        destination->clear();
        return;
    }

    switch(prefType) {
    case NetVariantTypeEnum_Bool:
        destination->setBool(source->toBool());
        return;
    case NetVariantTypeEnum_Char: {
        QString v = source->toString();
        if(v.isNull() || v.isEmpty()) {
            qDebug() << "Can't set empty string to char, setting to null.";
            destination->setChar(QChar::Null);
        } else {
            if(v.length() == 1) {
                destination->setChar(v.at(0));
            } else {
                qDebug() << "Can't set string to char that has more than one character.";
                destination->setChar(QChar::Null);
            }
        }
        return;
    }
    case NetVariantTypeEnum_Int:
    {
        int result = source->toInt(&ok);
        if(ok) {
            destination->setInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_UInt:
    {
        unsigned int result = source->toUInt(&ok);
        if(ok) {
            destination->setUInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_Double:
    {
        double result = source->toDouble(&ok);
        if(ok) {
            destination->setDouble(result);
            return;
        }
    }
    case NetVariantTypeEnum_String:
    {
        QString stringResult = source->toString();
        destination->setString(&stringResult);
        return;
    }
    case NetVariantTypeEnum_DateTime:
    {
        QDateTime dateTimeResult = source->toDateTime();
        if(!dateTimeResult.isValid()) {
            qDebug() << "Invalid date time";
            break;
        }
        if(dateTimeResult.isNull()) {
            destination->clear();
            break;
        }
        destination->setDateTime(dateTimeResult);
        return;
    }
    case NetVariantTypeEnum_Object:
    {
        if (source->type() == QMetaType::QObjectStar) {

            QObject* value = source->value<QObject*>();
            NetValueInterface* netValue = qobject_cast<NetValueInterface*>(value);
            if(netValue) {
                destination->setNetInstance(netValue->getNetInstance());
                return;
            }
        }
        break;
    }
    default:
        break;
    }

    switch(source->type()) {
    case QVariant::Invalid:
        destination->clear();
        break;
    case QVariant::Bool:
        destination->setBool(source->value<bool>());
        break;
    case QVariant::Char:
        destination->setChar(source->toChar());
        break;
    case QVariant::Int:
        destination->setInt(source->value<int>());
        break;
    case QVariant::UInt:
        destination->setUInt(source->value<unsigned int>());
        break;
    case QVariant::Double:
        destination->setDouble(source->value<double>());
        break;
    case QVariant::String:
    {
        QString stringValue = source->toString();
        destination->setString(&stringValue);
        break;
    }
    case QVariant::DateTime:
    {
        QDateTime dateTimeValue = source->toDateTime();
        destination->setDateTime(dateTimeValue);
        break;
    }
    default:

        if(source->userType() == qMetaTypeId<QJSValue>()) {
            // TODO: Either serialize this type to a string, to be deserialized in .NET, or
            // pass raw value to .NET to be dynamically invoked (using dynamic).
            // See qtdeclarative\src\plugins\qmltooling\qmldbg_debugger\qqmlenginedebugservice.cpp:184
            // for serialization methods.
        }

        qDebug() << "Unsupported variant type: " << source->type();
        break;
    }
}

QMetaObject *metaObjectFor(QSharedPointer<NetTypeInfo> typeInfo)
{
    if (typeInfo->metaObject) {
        return reinterpret_cast<QMetaObject *>(typeInfo->metaObject);
    }

    QMetaObjectBuilder mob;
    mob.setSuperClass(&QObject::staticMetaObject);
    mob.setClassName(typeInfo->getClassName().toLatin1());
    mob.setFlags(QMetaObjectBuilder::DynamicMetaObject);

    // register all the signals for the type

    if(typeInfo->getSignalCount() > 0) {
        for(uint index = 0; index <= typeInfo->getSignalCount() - 1; index++)
        {
            QSharedPointer<NetSignalInfo> signalInfo = typeInfo->getSignal(index);
            QString signature = signalInfo->getSignature();
            mob.addSignal(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()));
        }
    }

    // NOTE: It is important to register properties after the signals (before methods)
    // because of the assumptions we make about getting the "notifySignal" by index.

    if(typeInfo->getPropertyCount() > 0) {
        for(uint index = 0; index <= typeInfo->getPropertyCount() - 1; index++)
        {
            QSharedPointer<NetPropertyInfo> propertyInfo = typeInfo->getProperty(index);
            QMetaPropertyBuilder propb = mob.addProperty(propertyInfo->getPropertyName().toLatin1(),
                                                         "QVariant",
                                                         index);
            QSharedPointer<NetSignalInfo> notifySignal = propertyInfo->getNotifySignal();
            if(notifySignal != NULL) {
                // The signal was previously registered, find the index.
                for(uint signalIndex = 0; signalIndex <= typeInfo->getSignalCount() - 1; signalIndex++)
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
    }

    if(typeInfo->getMethodCount() > 0) {
        for(uint index = 0; index <= typeInfo->getMethodCount() - 1; index++)
        {
            QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getMethodInfo(index);
            QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
            QString signature = methodInfo->getSignature();
            if(returnType != NULL) {
                mob.addMethod(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()), "QVariant");
            } else {
                mob.addMethod(QMetaObject::normalizedSignature(signature.toLocal8Bit().constData()));
            }
        }
    }

    QMetaObject *mo = mob.toMetaObject();
    typeInfo->metaObject = mo;
    return mo;
}

NetValueMetaObject::NetValueMetaObject(QObject *value,
                                       QSharedPointer<NetInstance> instance) :
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

        QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
        readProperty(propertyInfo, instance, result);

        metaPackValue(result, reinterpret_cast<QVariant*>(a[0]));
    }
        break;
    case WriteProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        QSharedPointer<NetPropertyInfo> propertyInfo = instance->getTypeInfo()->getProperty(idx - offset);

        QSharedPointer<NetVariant> newValue = QSharedPointer<NetVariant>(new NetVariant());
        metaUnpackValue(newValue, reinterpret_cast<QVariant*>(a[0]), propertyInfo->getReturnType()->getPrefVariantType());

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
        if(idx < (int)instance->getTypeInfo()->getSignalCount()) {
            // This is a signal call, activate it!
            activate(value, idx + offset, a);
            return -1;
        }

        idx -= instance->getTypeInfo()->getSignalCount();

        QSharedPointer<NetMethodInfo> methodInfo = instance->getTypeInfo()->getMethodInfo(idx);

        QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());

        if(methodInfo->getParameterCount() > 0) {
            for(uint index = 0; index <= methodInfo->getParameterCount() - 1; index++)
            {
                QSharedPointer<NetMethodInfoArguement> parameter = methodInfo->getParameter(index);

                QSharedPointer<NetVariant> netVariant = QSharedPointer<NetVariant>(new NetVariant());
                metaUnpackValue(netVariant, reinterpret_cast<QVariant*>(a[index + 1]), parameter->getType()->getPrefVariantType());
                parameters->add(netVariant);
            }
        }

        QSharedPointer<NetVariant> result;
        if(methodInfo->getReturnType() != NULL) {
            result = QSharedPointer<NetVariant>(new NetVariant());
        }

        invokeNetMethod(methodInfo, instance, parameters, result);

        if(result != NULL) {
            metaPackValue(result, reinterpret_cast<QVariant*>(a[0]));
        }
    }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

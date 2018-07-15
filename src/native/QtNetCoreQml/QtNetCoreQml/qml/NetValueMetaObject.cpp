#include <QtNetCoreQml/qml/NetValueMetaObject.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/types/NetMethodInfo.h>
#include <QtNetCoreQml/types/NetPropertyInfo.h>
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
        NetValue* netValue = new NetValue(newInstance, NULL);
        QQmlEngine::setObjectOwnership(netValue, QQmlEngine::JavaScriptOwnership);
        destination->setValue(netValue);
        break;
    }
    default:
        qDebug() << "Unsupported variant type: " << source->getVariantType();
        break;
    }
}

void metaUnpackValue(NetVariant* destination, QVariant* source, NetVariantTypeEnum prefType) {
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

    if(typeInfo->getMethodCount() > 0) {
        for(uint index = 0; index <= typeInfo->getMethodCount() - 1; index++)
        {
            QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getMethodInfo(index);
            QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
            QString signature = methodInfo->getMethodName();

            signature.append("(");

            if(methodInfo->getParameterCount() > 0) {
                for(uint parameterIndex = 0; parameterIndex <= methodInfo->getParameterCount() - 1; parameterIndex++)
                {
                    if(parameterIndex > 0) {
                        signature.append(", ");
                    }
                    signature.append("QVariant");
                }
            }

            signature.append(")");

            if(returnType != NULL) {
                mob.addMethod(signature.toLocal8Bit().constData(), "QVariant");
            } else {
                mob.addMethod(signature.toLocal8Bit().constData());
            }
        }
    }

    if(typeInfo->getPropertyCount() > 0) {
        for(uint index = 0; index <= typeInfo->getPropertyCount() - 1; index++)
        {
            QSharedPointer<NetPropertyInfo> propertyInfo = typeInfo->getProperty(index);
            QMetaPropertyBuilder propb = mob.addProperty(propertyInfo->getPropertyName().toLatin1(),
                                                         "QVariant",
                                                         index);
            propb.setWritable(propertyInfo->canWrite());
            propb.setReadable(propertyInfo->canRead());
        }
    }

    QMetaObject *mo = mob.toMetaObject();
    typeInfo->metaObject = mo;
    return mo;
}

NetValueMetaObject::NetValueMetaObject(QObject *value,
                                       QSharedPointer<NetInstance> instance) :
    value(value),
    instance(instance),
    signalCount(0)
{

}

int NetValueMetaObject::metaCall(QMetaObject::Call c, int idx, void **a)
{
    return -1;
}

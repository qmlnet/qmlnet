#include "net_qml_meta.h"
#include "net_type_info.h"
#include "net_type_info_method.h"
#include "net_type_info_property.h"
#include "net_type_info_manager.h"
#include "net_instance.h"
#include "net_variant.h"
#include <private/qmetaobjectbuilder_p.h>
#include <QDebug>
#include "net_qml_value.h"
#include <QQmlEngine>
#include <QDateTime>

void metaPackValue(NetVariant* source, QVariant* destination) {
    switch(source->GetVariantType()) {
    case NetVariantTypeEnum_Invalid:
        destination->clear();
        break;
    case NetVariantTypeEnum_Bool:
        destination->setValue(source->GetBool());
        break;
    case NetVariantTypeEnum_Int:
        destination->setValue(source->GetInt());
        break;
    case NetVariantTypeEnum_Double:
        destination->setValue(source->GetDouble());
        break;
    case NetVariantTypeEnum_String:
        destination->setValue(source->GetString());
        break;
    case NetVariantTypeEnum_DateTime:
        destination->setValue(source->GetDateTime());
        break;
    case NetVariantTypeEnum_Object:
    {
        NetInstance* newInstance = source->GetNetInstance()->Clone();
        NetValue* netValue = new NetValue(newInstance, NULL);
        QQmlEngine::setObjectOwnership(netValue, QQmlEngine::JavaScriptOwnership);
        destination->setValue(netValue);
        break;
    }
    default:
        qDebug() << "Unsupported variant type: " << source->GetVariantType();
        break;
    }
}

void metaUnpackValue(NetVariant* destination, QVariant* source, NetVariantTypeEnum prefType) {
    bool ok = false;

    if(source->isNull()) {
        destination->Clear();
        return;
    }

    switch(prefType) {
    case NetVariantTypeEnum_Bool:
        destination->SetBool(source->toBool());
        return;
    case NetVariantTypeEnum_Int:
    {
        int result = source->toInt(&ok);
        if(ok) {
            destination->SetInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_Double:
    {
        double result = source->toDouble(&ok);
        if(ok) {
            destination->SetDouble(result);
            return;
        }
    }
    case NetVariantTypeEnum_String:
    {
        QString stringResult = source->toString();
        destination->SetString(&stringResult);
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
            destination->Clear();
            break;
        }
        destination->SetDateTime(dateTimeResult);
        return;
    }
    case NetVariantTypeEnum_Object:
    {
        if (source->type() == QMetaType::QObjectStar) {

            QObject* value = source->value<QObject*>();
            NetValueInterface* netValue = qobject_cast<NetValueInterface*>(value);
            if(netValue) {
                destination->SetNetInstance(netValue->GetNetInstance()->Clone());
                return;
            }
        }
        break;
    }
    }

    switch(source->type()) {
    case QVariant::Invalid:
        destination->Clear();
        break;
    case QVariant::Bool:
        destination->SetBool(source->value<bool>());
        break;
    case QVariant::Double:
        destination->SetDouble(source->value<double>());
        break;
    case QVariant::Int:
        destination->SetDouble(source->value<int>());
        break;
    case QVariant::String:
    {
        QString stringValue = source->toString();
        destination->SetString(&stringValue);
        break;
    }
    case QVariant::DateTime:
    {
        QDateTime dateTimeValue = source->toDateTime();
        destination->SetDateTime(dateTimeValue);
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

QMetaObject *metaObjectFor(NetTypeInfo *typeInfo)
{
    if (typeInfo->metaObject) {
        return reinterpret_cast<QMetaObject *>(typeInfo->metaObject);
    }

    QMetaObjectBuilder mob;
    mob.setSuperClass(&QObject::staticMetaObject);
    mob.setClassName(typeInfo->GetClassName().c_str());
    mob.setFlags(QMetaObjectBuilder::DynamicMetaObject);

    for(int index = 0; index <= typeInfo->GetMethodCount() - 1; index++)
    {
        NetMethodInfo* methodInfo = typeInfo->GetMethod(index);
        NetTypeInfo* returnType = methodInfo->GetReturnType();
        QString signature = QString::fromStdString(methodInfo->GetMethodName());

        signature.append("(");

        for(int parameterIndex = 0; parameterIndex <= methodInfo->GetParameterCount() - 1; parameterIndex++)
        {
            if(parameterIndex > 0) {
                signature.append(", ");
            }

            std::string parameterName;
            NetTypeInfo* parameterType = NULL;
            methodInfo->GetParameterInfo(0, &parameterName, &parameterType);
            signature.append("QVariant");
        }

        signature.append(")");

        if(returnType) {
            mob.addMethod(signature.toLocal8Bit().constData(), "QVariant");
        } else {
            mob.addMethod(signature.toLocal8Bit().constData());
        }
    }

    for(int index = 0; index <= typeInfo->GetPropertyCount() - 1; index++)
    {
        NetPropertyInfo* propertyInfo = typeInfo->GetProperty(index);
        QMetaPropertyBuilder propb = mob.addProperty(propertyInfo->GetPropertyName().c_str(),
                                                     "QVariant",
                                                     index);
        propb.setWritable(propertyInfo->CanWrite());
        propb.setReadable(propertyInfo->CanRead());
    }

    QMetaObject *mo = mob.toMetaObject();

    typeInfo->metaObject = mo;
    return mo;
}

GoValueMetaObject::GoValueMetaObject(QObject *value, NetInstance *instance)
    : value(value), instance(instance)
{
    *static_cast<QMetaObject *>(this) = *metaObjectFor(instance->GetTypeInfo());

    QObjectPrivate *objPriv = QObjectPrivate::get(value);
    objPriv->metaObject = this;
}

int GoValueMetaObject::metaCall(QMetaObject::Call c, int idx, void **a)
{
    switch(c) {
    case ReadProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        NetPropertyInfo* propertyInfo = instance->GetTypeInfo()->GetProperty(idx - offset);

        NetVariant* result = NetTypeInfoManager::ReadProperty(propertyInfo, instance);

        metaPackValue(result, reinterpret_cast<QVariant*>(a[0]));

        delete result;
    }
        break;
    case WriteProperty:
    {
        int offset = propertyOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        NetPropertyInfo* propertyInfo = instance->GetTypeInfo()->GetProperty(idx - offset);

        NetVariant* newValue = new NetVariant();
        metaUnpackValue(newValue, reinterpret_cast<QVariant*>(a[0]), propertyInfo->GetReturnType()->GetPrefVariantType());

        NetTypeInfoManager::WriteProperty(propertyInfo, instance, newValue);

        delete newValue;
    }
        break;
    case  InvokeMetaMethod:
    {
        int offset = methodOffset();
        if (idx < offset) {
            return value->qt_metacall(c, idx, a);
        }

        NetMethodInfo* methodInfo = instance->GetTypeInfo()->GetMethod(idx - offset);

        std::vector<NetVariant*> parameters;

        for(int index = 0; index <= methodInfo->GetParameterCount() - 1; index++)
        {
            NetTypeInfo* typeInfo = NULL;
            methodInfo->GetParameterInfo(index, NULL, &typeInfo);

            NetVariant* netVariant = new NetVariant();
            metaUnpackValue(netVariant, reinterpret_cast<QVariant*>(a[index + 1]), typeInfo->GetPrefVariantType());
            parameters.insert(parameters.end(), netVariant);
        }

        NetVariant* result = NetTypeInfoManager::InvokeMethod(methodInfo, instance, parameters);

        for(int x = 0; x < parameters.size(); x++) {
            NetVariant* variant = parameters.at(x);
            delete variant;
        }

        if(result) {
            metaPackValue(result, reinterpret_cast<QVariant*>(a[0]));
        }

        delete result;
    }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

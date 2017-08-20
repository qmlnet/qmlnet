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

void packValue(NetVariant* source, void* destination) {
    QVariant *casted = reinterpret_cast<QVariant *>(destination);
    switch(source->GetVariantType()) {
    case NetVariantTypeEnum_Invalid:
        casted->clear();
        break;
    case NetVariantTypeEnum_Bool:
        casted->setValue(source->GetBool());
        break;
    case NetVariantTypeEnum_Int:
        casted->setValue(source->GetInt());
    case NetVariantTypeEnum_Double:
        casted->setValue(source->GetDouble());
        break;
    case NetVariantTypeEnum_String:
        casted->setValue(source->GetString());
        break;
    case NetVariantTypeEnum_DateTime:
        casted->setValue(source->GetDateTime());
        break;
    case NetVariantTypeEnum_Object:
    {
        NetInstance* newInstance = source->GetNetInstance()->Clone();
        NetValue* netValue = new NetValue(newInstance, NULL);
        QQmlEngine::setObjectOwnership(netValue, QQmlEngine::JavaScriptOwnership);
        casted->setValue(netValue);
        break;
    }
    default:
        qDebug() << "Unsupported variant type: " << source->GetVariantType();
        break;
    }
}

void unpackValue(NetVariant* destination, void* source, NetVariantTypeEnum prefType) {
    QVariant *casted = reinterpret_cast<QVariant *>(source);
    bool ok = false;

    if(casted->isNull()) {
        destination->Clear();
        return;
    }

    switch(prefType) {
    case NetVariantTypeEnum_Bool:
        destination->SetBool(casted->toBool());
        return;
    case NetVariantTypeEnum_Int:
    {
        int result = casted->toInt(&ok);
        if(ok) {
            destination->SetInt(result);
            return;
        }
        break;
    }
    case NetVariantTypeEnum_Double:
    {
        double result = casted->toDouble(&ok);
        if(ok) {
            destination->SetDouble(result);
            return;
        }
    }
    case NetVariantTypeEnum_String:
    {
        QString stringResult = casted->toString();
        destination->SetString(&stringResult);
        return;
    }
    case NetVariantTypeEnum_DateTime:
    {
        QDateTime dateTimeResult = casted->toDateTime();
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
    }

    switch(casted->type()) {
    case QVariant::Invalid:
        destination->Clear();
        break;
    case QVariant::Bool:
        destination->SetBool(casted->value<bool>());
        break;
    case QVariant::Double:
        destination->SetDouble(casted->value<double>());
    case QVariant::Int:
        destination->SetDouble(casted->value<int>());
        break;
    case QVariant::String:
    {
        QString stringValue = casted->toString();
        destination->SetString(&stringValue);
        break;
    }
    case QVariant::DateTime:
    {
        QDateTime dateTimeValue = casted->toDateTime();
        destination->SetDateTime(dateTimeValue);
        break;
    }
    default:
        qDebug() << "Unsupported variant type: " << casted->type();
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
    mob.setClassName("TestQmlImport");
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

        packValue(result, a[0]);

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
        unpackValue(newValue, a[0], propertyInfo->GetReturnType()->GetPrefVariantType());

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
            unpackValue(netVariant, a[index + 1], typeInfo->GetPrefVariantType());
            parameters.insert(parameters.end(), netVariant);
        }

        NetVariant* result = NetTypeInfoManager::InvokeMethod(methodInfo, instance, parameters);

        if(result) {
            packValue(result, a[0]);
        }

        delete result;
    }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

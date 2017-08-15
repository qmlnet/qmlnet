#include "net_qml_meta.h"
#include "net_type_info.h"
#include "net_type_info_method.h"
#include "net_type_info_property.h"
#include "net_type_info_manager.h"
#include "net_instance.h"
#include <private/qmetaobjectbuilder_p.h>
#include <QDebug>

const char* cppTypeNameFromInterType(NetInterTypeEnum interType) {
    switch(interType) {
    case NetInterTypeEnum_Bool:
        return "bool";
    case NetInterTypeEnum_Int:
        return "int";
    case NetInterTypeEnum_Double:
        return "double";
    case NetInterTypeEnum_Float:
        return "float";
    case NetInterTypeEnum_String:
        return "QString";
    case NetInterTypeEnum_Date:
        return "QDate";
    case NetInterTypeEnum_Object:
        return "QObject";
    }
    qDebug() << "Unsupported intertype: " << interType;
    return NULL;
}

void packValue(NetInstance* instance, void* value) {
    switch(instance->GetInterType()) {
    case NetInterTypeEnum_Bool: {
        bool *in = reinterpret_cast<bool *>(value);
        instance->SetBool(*in);
        break;
    }
    default:
        qDebug() << "Unsupported intertype: " << instance->GetInterType();
        break;
    }
}

void unpackValue(NetInstance* instance, void* value) {
    switch(instance->GetInterType()) {
    case NetInterTypeEnum_Bool: {
        bool *out = reinterpret_cast<bool *>(value);
        *out = instance->GetBool();
        break;
    }
    default:
        qDebug() << "Unsupported intertype: " << instance->GetInterType();
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
            signature.append(cppTypeNameFromInterType(parameterType->GetInterType()));
        }

        signature.append(")");

        if(returnType) {
            mob.addMethod(signature.toLocal8Bit().constData(), cppTypeNameFromInterType(returnType->GetInterType()));
        } else {
            mob.addMethod(signature.toLocal8Bit().constData());
        }
    }

    for(int index = 0; index <= typeInfo->GetPropertyCount() - 1; index++)
    {
        NetPropertyInfo* propertyInfo = typeInfo->GetProperty(index);
        QMetaPropertyBuilder propb = mob.addProperty(propertyInfo->GetPropertyName().c_str(),
            cppTypeNameFromInterType(propertyInfo->GetReturnType()->GetInterType()),
            index);
        propb.setWritable(propertyInfo->CanWrite());
        propb.setReadable(propertyInfo->CanRead());
    }

    QMetaObject *mo = mob.toMetaObject();

    typeInfo->metaObject = mo;
    return mo;
}

GoValueMetaObject::GoValueMetaObject(QObject *value, NetInstance *instance, NetTypeInfo *typeInfo)
    : value(value), instance(instance), typeInfo(typeInfo)
{
    *static_cast<QMetaObject *>(this) = *metaObjectFor(typeInfo);

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

            NetPropertyInfo* propertyInfo = typeInfo->GetProperty(idx - offset);

            NetInstance* result = NetTypeInfoManager::ReadProperty(propertyInfo, instance);

            unpackValue(result, a[0]);
        }
        break;
    case WriteProperty:
        {
            int offset = propertyOffset();
            if (idx < offset) {
                return value->qt_metacall(c, idx, a);
            }

            NetPropertyInfo* propertyInfo = typeInfo->GetProperty(idx - offset);

            NetInstance* newValue = new NetInstance(propertyInfo->GetReturnType()->GetInterType());
            packValue(newValue, a[0]);

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

            NetMethodInfo* methodInfo = typeInfo->GetMethod(idx - offset);

            std::vector<NetInstance*> parameters;

            for(int index = 0; index <= methodInfo->GetParameterCount() - 1; index++)
            {
                NetTypeInfo* typeInfo = NULL;
                methodInfo->GetParameterInfo(index, NULL, &typeInfo);

                NetInstance* instance = new NetInstance(typeInfo->GetInterType());
                packValue(instance, a[index + 1]);

                parameters.insert(parameters.end(), instance);
            }

            NetInstance* result = NetTypeInfoManager::InvokeMethod(methodInfo, instance, parameters);

            if(result) {
                unpackValue(result, a[0]);
            }
        }
        break;
    default:
        break; // Unhandled.
    }

    return -1;
}

void GoValueMetaObject::activatePropIndex(int propIndex)
{
    // Properties are added first, so the first fieldLen methods are in
    // fact the signals of the respective properties.
    int relativeIndex = propIndex - propertyOffset();
    activate(value, methodOffset() + relativeIndex, 0);
}

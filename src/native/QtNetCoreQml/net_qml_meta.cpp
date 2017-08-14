#include "net_qml_meta.h"
#include "net_type_info.h"
#include "net_type_info_method.h"
#include "net_type_info_property.h"
#include "net_type_info_manager.h"
#include "net_instance.h"
#include <private/qmetaobjectbuilder_p.h>
#include <QDebug>

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
            std::string parameterName;
            NetTypeInfo* parameterType = NULL;
            methodInfo->GetParameterInfo(0, &parameterName, &parameterType);
            signature.append(parameterType->GetTypeName().c_str());
        }
        signature.append(")");
        if(returnType) {
            mob.addMethod(signature.toLocal8Bit().constData());
        } else {
            mob.addMethod(signature.toLocal8Bit().constData());
        }
    }

    for(int index = 0; index <= typeInfo->GetPropertyCount() - 1; index++)
    {
        NetPropertyInfo* propertyInfo = typeInfo->GetProperty(index);
        QMetaPropertyBuilder propb = mob.addProperty(propertyInfo->GetPropertyName().c_str(), "bool", index);
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
            int propOffset = propertyOffset();
            if (idx < propOffset) {
                return value->qt_metacall(c, idx, a);
            }

            NetPropertyInfo* propertyInfo = typeInfo->GetProperty(idx - 1);

            switch(propertyInfo->GetReturnType()->GetInterType())
            {
            case NetInterTypeEnum_Bool:
                {
                    bool *out = reinterpret_cast<bool *>(a[0]);
                    NetInstance* result = NetTypeInfoManager::ReadProperty(propertyInfo, instance);
                    *out = result->GetBool();
                    delete result;
                }
                break;
            default:
                qDebug() << "Unsupported inter type: " << propertyInfo->GetReturnType()->GetInterType();
                break;
            }
        }
        break;
    case WriteProperty:
        {
            int propOffset = propertyOffset();
            if (idx < propOffset) {
                return value->qt_metacall(c, idx, a);
            }

            NetPropertyInfo* propertyInfo = typeInfo->GetProperty(idx - 1);

            switch(propertyInfo->GetReturnType()->GetInterType())
            {
            case NetInterTypeEnum_Bool:
                {
                    bool *out = reinterpret_cast<bool *>(a[0]);
                    NetInstance* newValue = new NetInstance(NetInterTypeEnum_Bool);
                    newValue->SetBool(*out);
                    NetTypeInfoManager::WriteProperty(propertyInfo, instance, newValue);
                    delete newValue;
                }
                break;
            default:
                qDebug() << "Unsupported inter type: " << propertyInfo->GetReturnType()->GetInterType();
                break;
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

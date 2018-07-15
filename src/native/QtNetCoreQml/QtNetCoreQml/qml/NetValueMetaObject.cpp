#include <QtNetCoreQml/qml/NetValueMetaObject.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/types/NetMethodInfo.h>
#include <QtNetCoreQml/types/NetPropertyInfo.h>
#include <QQmlEngine>
#include <QDebug>
#include <private/qmetaobjectbuilder_p.h>

void metaPackValue(QSharedPointer<NetVariant> source, QVariant* destination) {

}

void metaUnpackValue(NetVariant* destination, QVariant* source, NetVariantTypeEnum prefType) {

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
        for(int index = 0; index <= typeInfo->getMethodCount() - 1; index++)
        {
            QSharedPointer<NetMethodInfo> methodInfo = typeInfo->getMethodInfo(index);
            QSharedPointer<NetTypeInfo> returnType = methodInfo->getReturnType();
            QString signature = methodInfo->getMethodName();

            signature.append("(");

            for(int parameterIndex = 0; parameterIndex <= methodInfo->getParameterCount() - 1; parameterIndex++)
            {
                if(parameterIndex > 0) {
                    signature.append(", ");
                }
                signature.append("QVariant");
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
        for(int index = 0; index <= typeInfo->getPropertyCount() - 1; index++)
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

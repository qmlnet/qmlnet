#ifndef NETVALUETYPE_H
#define NETVALUETYPE_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/qml/NetValue.h>
#include <QtNetCoreQml/qml/NetValueMetaObject.h>
#include <QtNetCoreQml/types/Callbacks.h>

template <int N>
class NetValueType : public NetValue
{
public:

    NetValueType()
        : NetValue(instantiateType(typeInfo), nullptr) {}

    static void init(QSharedPointer<NetTypeInfo> info)
    {
        typeInfo = info;
        static_cast<QMetaObject &>(staticMetaObject) = *metaObjectFor(typeInfo);
    }

    static QSharedPointer<NetTypeInfo> typeInfo;
    static QMetaObject staticMetaObject;
};

template <int N>
QSharedPointer<NetTypeInfo> NetValueType<N>::typeInfo = NULL;
template <int N>
QMetaObject NetValueType<N>::staticMetaObject = QMetaObject();

#endif // NETVALUETYPE_H

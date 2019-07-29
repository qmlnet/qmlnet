#ifndef NETVALUETYPE_H
#define NETVALUETYPE_H

#include <QmlNet.h>
#include <QmlNet/qml/NetValue.h>
#include <QmlNet/qml/NetValueMetaObject.h>
#include <QmlNet/types/Callbacks.h>
#include <QQmlEngine>

template <int N>
class NetValueType : public NetValue
{
public:

    NetValueType()
        : NetValue(QmlNet::instantiateType(typeInfo, -1), nullptr) {}

    static void init(QSharedPointer<NetTypeInfo> info)
    {
        typeInfo = info;
        static_cast<QMetaObject &>(staticMetaObject) = *metaObjectFor(typeInfo);
    }

    static QObject *build(QQmlEngine *engine, QJSEngine *scriptEngine)
    {
        Q_UNUSED(engine)
        Q_UNUSED(scriptEngine)
        return new NetValueType<N>();
    }

    static QSharedPointer<NetTypeInfo> typeInfo;
    static QMetaObject staticMetaObject;
};

template <int N>
QSharedPointer<NetTypeInfo> NetValueType<N>::typeInfo = nullptr;
template <int N>
QMetaObject NetValueType<N>::staticMetaObject = QMetaObject();

#endif // NETVALUETYPE_H

#include <QmlNet/qml/NetQObject.h>
#include <QmlNet/qml/NetQObjectSignalConnection.h>
#include <QmlNet/qml/NetQObjectArg.h>
#include <QmlNet/types/NetDelegate.h>
#include <QmlNet/types/Callbacks.h>
#include <QMetaMethod>
#include <QDebug>

NetQObject::NetQObject(QObject* qObject, bool ownsObject) :
    _qObject(qObject),
    _ownsObject(ownsObject)
{

}

NetQObject::~NetQObject()
{
    if(_ownsObject) {
        _qObject->deleteLater();
        _qObject = nullptr;
    }
}

QObject* NetQObject::getQObject()
{
    return _qObject;
}

QSharedPointer<NetVariant> NetQObject::getProperty(QString propertyName, bool* wasSuccess)
{
    QSharedPointer<NetVariant> netVariant;
    QByteArray propertyNameLocal = propertyName.toLocal8Bit();

    if(_qObject->metaObject()->indexOfProperty(propertyNameLocal.data()) == -1) {
        if(wasSuccess) {
            *wasSuccess = false;
        }
        return nullptr;
    }

    QVariant result = _qObject->property(propertyNameLocal.data());
    if(!result.isNull()) {
        netVariant = QSharedPointer<NetVariant>(new NetVariant());
        NetVariant::fromQVariant(&result, netVariant);
    }
    if(wasSuccess) {
        *wasSuccess = true;
    }
    return netVariant;
}

void NetQObject::setProperty(QString propertyName, QSharedPointer<NetVariant> value, bool* wasSuccess)
{
    bool result;
    if(value == nullptr) {
        result = _qObject->setProperty(propertyName.toLocal8Bit().data(), QVariant());
    } else {
        result = _qObject->setProperty(propertyName.toLocal8Bit().data(), value->toQVariant());
    }
    if(wasSuccess) {
        *wasSuccess = result;
    }
}

QSharedPointer<NetVariant> NetQObject::invokeMethod(QString methodName, QSharedPointer<NetVariantList> parameters, bool* wasSuccess)
{
    int parameterCount = 0;
    if(parameters != nullptr) {
        parameterCount = parameters->count();
    }

    // Find the best method
    int methodIndex = -1;
    QMetaMethod method;
    for(int x = 0; x < _qObject->metaObject()->methodCount(); x++) {
        method = _qObject->metaObject()->method(x);
        if(method.methodType() == QMetaMethod::Slot || method.methodType() == QMetaMethod::Method || method.methodType() == QMetaMethod::Signal) {
            if(methodName.compare(method.name()) == 0) {
                // make sure number of parameters match
                if(method.parameterCount() == parameterCount) {
                    methodIndex = x;
                    break;
                }
            }
        }
    }

    if(methodIndex == -1) {
        if(wasSuccess) {
            *wasSuccess = false;
        }
        return nullptr;
    }

    if(parameterCount > 10) {
        qWarning() << "Attempting to invoke a method with too many parameters: current: " << parameters->count() << " expected: <=10";
        if(wasSuccess) {
            *wasSuccess = false;
        }
        return nullptr;
    }

    int returnType = method.returnType();
    if(returnType == QMetaType::UnknownType) {
        qWarning() << "Unable to return type" << method.typeName() << ", it wasn't registered with the meta object system";
        if(wasSuccess) {
            *wasSuccess = false;
        }
        return nullptr;
    }

    NetQObjectArg returnValue(returnType);
    NetQObjectArg val0;
    NetQObjectArg val1;
    NetQObjectArg val2;
    NetQObjectArg val3;
    NetQObjectArg val4;
    NetQObjectArg val5;
    NetQObjectArg val6;
    NetQObjectArg val7;
    NetQObjectArg val8;
    NetQObjectArg val9;

    if(parameterCount >= 1) {
        val0 = NetQObjectArg(method.parameterType(0), parameters->get(0));
    }
    if(parameterCount >= 2) {
        val1 = NetQObjectArg(method.parameterType(1), parameters->get(1));
    }
    if(parameterCount >= 3) {
        val2 = NetQObjectArg(method.parameterType(2), parameters->get(2));
    }
    if(parameterCount >= 4) {
        val3 = NetQObjectArg(method.parameterType(3), parameters->get(3));
    }
    if(parameterCount >= 5) {
        val4 = NetQObjectArg(method.parameterType(4), parameters->get(4));
    }
    if(parameterCount >= 6) {
        val5 = NetQObjectArg(method.parameterType(5), parameters->get(5));
    }
    if(parameterCount >= 7) {
        val6 = NetQObjectArg(method.parameterType(6), parameters->get(6));
    }
    if(parameterCount >= 8) {
        val7 = NetQObjectArg(method.parameterType(7), parameters->get(7));
    }
    if(parameterCount >= 9) {
        val8 = NetQObjectArg(method.parameterType(8), parameters->get(8));
    }
    if(parameterCount >= 10) {
        val9 = NetQObjectArg(method.parameterType(9), parameters->get(9));
    }

    if(!method.invoke(_qObject,
                      Qt::DirectConnection,
                      returnValue.genericReturnArguemnet(),
                      val0.genericArgument(),
                      val1.genericArgument(),
                      val2.genericArgument(),
                      val3.genericArgument(),
                      val4.genericArgument(),
                      val5.genericArgument(),
                      val6.genericArgument(),
                      val7.genericArgument(),
                      val8.genericArgument(),
                      val9.genericArgument())) {
        if(wasSuccess) {
            *wasSuccess = false;
        }
        return nullptr;
    }

    if(wasSuccess) {
        *wasSuccess = true;
    }

    returnValue.unpack();
    return returnValue.getNetVariant();
}

QSharedPointer<NetQObjectSignalConnection> NetQObject::attachSignal(QString signalName, QSharedPointer<NetReference> delegate, bool* wasSucessful)
{
    int signalMethodIndex = -1;
    QMetaMethod signalMethod;
    for(int x = 0; x <_qObject->metaObject()->methodCount(); x++) {
        signalMethod = _qObject->metaObject()->method(x);
        if(signalMethod.methodType() == QMetaMethod::Signal) {
            if(signalName.compare(signalMethod.name()) == 0) {
                signalMethodIndex = x;
                break;
            }
        }
    }

    // If signal not found, dump the registered signals for debugging.
    if(signalMethodIndex < 0) {
        if(wasSucessful) {
            *wasSucessful = false;
        }
        return nullptr;
    }

    QSharedPointer<NetQObjectSignalConnection> signalConnection = QSharedPointer<NetQObjectSignalConnection>(new NetQObjectSignalConnection(delegate, _qObject));
    QMetaObject::Connection connection = QObject::connect(_qObject,
                                                          signalMethod,
                                                          signalConnection.data(),
                                                          signalConnection->getSignalHandler());

    if(!connection) {
        if(wasSucessful) {
            *wasSucessful = false;
        }
        return nullptr;
    }

    if(wasSucessful) {
        *wasSucessful = true;
    }

    return signalConnection;
}

QSharedPointer<NetQObjectSignalConnection> NetQObject::attachNotifySignal(QString propertyName, QSharedPointer<NetReference> delegate, bool* wasSuccessful)
{
    int propertyIndex = _qObject->metaObject()->indexOfProperty(propertyName.toLocal8Bit().data());
    if(propertyIndex == -1) {
        if(wasSuccessful) {
            *wasSuccessful = false;
        }
        return nullptr;
    }

    QMetaProperty property = _qObject->metaObject()->property(propertyIndex);

    if(property.notifySignalIndex() == -1) {
        if(wasSuccessful) {
            *wasSuccessful = false;
        }
        return nullptr;
    }

    QSharedPointer<NetQObjectSignalConnection> signalConnection = QSharedPointer<NetQObjectSignalConnection>(new NetQObjectSignalConnection(delegate, _qObject));
    QMetaObject::Connection connection = QObject::connect(_qObject,
                                                          property.notifySignal(),
                                                          signalConnection.data(),
                                                          signalConnection->getSignalHandler());

    if(!connection) {
        if(wasSuccessful) {
            *wasSuccessful = false;
        }
        return nullptr;
    }

    if(wasSuccessful) {
        *wasSuccessful = true;
    }

    return signalConnection;
}

QSharedPointer<NetQObject> NetQObject::buildQObject(QString className, QSharedPointer<NetVariantList> constructorParameters)
{
    int typeId = QMetaType::type(className.toLocal8Bit().data());
    if(typeId == QMetaType::UnknownType) {
        qWarning() << "The class" << className << "isn't known to the meta type system.";
        return nullptr;
    }
    if((QMetaType::typeFlags(typeId) & QMetaType::PointerToQObject) == 0) {
        qWarning() << "The type" << className << "isn't a pointer to a QObject.";
        return nullptr;
    }

    if(constructorParameters != nullptr) {
        if(constructorParameters->count() > 0) {
            qWarning() << "Support for contstructor parameters isn't supported yet.";
            return nullptr;
        }
    }

    const QMetaObject* metaObject = QMetaType::metaObjectForType(typeId);
    QObject* result = metaObject->newInstance();

    if(result == nullptr) {
        qWarning() << "Couldn't create instance of" << className;
        return nullptr;
    }

    return QSharedPointer<NetQObject>(new NetQObject(result, true));
}

extern "C" {

Q_DECL_EXPORT void net_qobject_destroy(NetQObjectContainer* qObjectContainer)
{
    delete qObjectContainer;
}

Q_DECL_EXPORT NetVariantContainer* net_qobject_getProperty(NetQObjectContainer* qObjectContainer, const QChar* propertyName, uchar* result)
{
    bool wasSuccesful = false;
    auto value = qObjectContainer->qObject->getProperty(QString(propertyName), &wasSuccesful);
    if(wasSuccesful) {
        *result = 1;
    } else {
        *result = 0;
    }
    if(value == nullptr) {
        return nullptr;
    }
    return new NetVariantContainer{ value };
}

Q_DECL_EXPORT void net_qobject_setProperty(NetQObjectContainer* qObjectContainer, const QChar* propertyName, NetVariantContainer* netVariantContainer, uchar* result)
{
    bool wasSuccesful = false;
    if(netVariantContainer == nullptr) {
        qObjectContainer->qObject->setProperty(QString(propertyName), nullptr, &wasSuccesful);
    } else {
        qObjectContainer->qObject->setProperty(QString(propertyName), netVariantContainer->variant, &wasSuccesful);
    }
    if(wasSuccesful) {
        *result = 1;
    } else {
        *result = 0;
    }
}

Q_DECL_EXPORT NetVariantContainer* net_qobject_invokeMethod(NetQObjectContainer* qObjectContainer, const QChar* methodName, NetVariantListContainer* parametersContainer, uchar* result)
{
    QSharedPointer<NetVariantList> parameters = nullptr;
    if(parametersContainer != nullptr) {
        parameters = parametersContainer->list;
    }
    bool wasSuccesful = false;
    auto value = qObjectContainer->qObject->invokeMethod(QString(methodName), parameters, &wasSuccesful);
    if(wasSuccesful) {
        *result = 1;
    } else {
        *result = 0;
    }
    if(value == nullptr) {
        return nullptr;
    }
    return new NetVariantContainer { value };
}

Q_DECL_EXPORT NetQObjectSignalConnectionContainer* net_qobject_attachSignal(NetQObjectContainer* qObjectContainer, const QChar* signalName, NetReferenceContainer* delegate, uchar* result)
{
    bool wasSuccesful = false;
    auto signalConnection = qObjectContainer->qObject->attachSignal(QString(signalName), delegate->instance, &wasSuccesful);
    if(wasSuccesful) {
        *result = 1;
    } else {
        *result = 0;
    }
    if(signalConnection == nullptr) {
        return nullptr;
    }
    return new NetQObjectSignalConnectionContainer { signalConnection };
}

Q_DECL_EXPORT NetQObjectSignalConnectionContainer* net_qobject_attachNotifySignal(NetQObjectContainer* qObjectContainer, const QChar* propertyName, NetReferenceContainer* delegate, uchar* result)
{
    bool wasSuccesful = false;
    auto signalConnection = qObjectContainer->qObject->attachNotifySignal(QString(propertyName), delegate->instance, &wasSuccesful);
    if(wasSuccesful) {
        *result = 1;
    } else {
        *result = 0;
    }
    if(signalConnection == nullptr) {
        return nullptr;
    }
    return new NetQObjectSignalConnectionContainer { signalConnection };
}

Q_DECL_EXPORT NetQObjectContainer* net_qobject_buildQObject(const QChar* className, NetVariantListContainer* constructorParameters)
{
    QSharedPointer<NetVariantList> params;
    if(constructorParameters) {
        params = constructorParameters->list;
    }
    QSharedPointer<NetQObject> result = NetQObject::buildQObject(QString(className), params);
    if(result == nullptr) {
        return nullptr;
    }
    return new NetQObjectContainer { result };
}

Q_DECL_EXPORT void net_qobject_signal_handler_destroy(NetQObjectSignalConnectionContainer* signalContainer)
{
    delete signalContainer;
}

}

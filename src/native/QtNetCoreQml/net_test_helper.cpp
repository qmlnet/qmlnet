#include "net_test_helper.h"
#include "net_qml_meta.h"
#include <QQmlComponent>
#include "net_type_info_method.h"
#include "net_qml_value.h"

void NetTestHelper::RunQml(QQmlApplicationEngine* qmlApplicationEngine, QString& qml)
{
    QQmlComponent component(qmlApplicationEngine);
    component.setData(qml.toUtf8(), QUrl());
    QObject *object = component.create();

    if(object == NULL) {
        qDebug() << "Couldn't create qml object.";
        return;
    }

    delete object;
}

void NetTestHelper::RunQmlMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, QString& methodName, NetVariant* parameter, NetVariant* result)
{
    QQmlComponent component(qmlApplicationEngine);
    component.setData(qml.toUtf8(), QUrl());
    QObject *object = component.create();

    if(object == NULL) {
        qDebug() << "Couldn't create qml object.";
        return;
    }

    NetValue* netValue = reinterpret_cast<NetValue*>(object);
    NetMethodInfo* methodInfo = NULL;

    for(int x = 0; x < netValue->GetNetInstance()->GetTypeInfo()->GetMethodCount(); x++) {
        NetMethodInfo* possibleMethodInfo = netValue->GetNetInstance()->GetTypeInfo()->GetMethod(x);
        if(strcmp(possibleMethodInfo->GetMethodName().c_str(), methodName.toLocal8Bit()) == 0) {
            methodInfo = possibleMethodInfo;
            break;
        }
    }

    if(!methodInfo) {
        qDebug() << "Couldn't find method: " << methodName;
        return;
    }

    QVariant returnedValue;
    QVariant parameterValue;

    if(methodInfo->GetReturnType()) {
        if(parameter) {
            metaPackValue(parameter, &parameterValue);
            QMetaObject::invokeMethod(object, methodInfo->GetMethodName().c_str(),
                Q_RETURN_ARG(QVariant, returnedValue),
                Q_ARG(QVariant, parameterValue));
        } else {
            QMetaObject::invokeMethod(object, methodInfo->GetMethodName().c_str(),
                Q_RETURN_ARG(QVariant, returnedValue));
        }
        metaUnpackValue(result, &returnedValue, methodInfo->GetReturnType()->GetPrefVariantType());
    } else {
        if(parameter) {
            metaPackValue(parameter, &parameterValue);
            QMetaObject::invokeMethod(object, methodInfo->GetMethodName().c_str(),
                Q_ARG(QVariant, parameterValue));
        } else {
            QMetaObject::invokeMethod(object, methodInfo->GetMethodName().c_str());
        }
    }

    delete object;
}

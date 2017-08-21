#include "net_test_helper.h"
#include "net_qml_meta.h"
#include <QQmlComponent>
#include "net_type_info_method.h"

void NetTestHelper::RunMethod(QQmlApplicationEngine* qmlApplicationEngine, QString& qml, NetMethodInfo* methodInfo, NetVariant* parameter, NetVariant* result)
{
    QQmlComponent component(qmlApplicationEngine);
    component.setData(qml.toUtf8(), QUrl());
    QObject *object = component.create();

//    QVariant returnedValue;
//    QVariant msg = "Hello from C++";
//    QMetaObject::invokeMethod(object, "myQmlFunction",
//            Q_RETURN_ARG(QVariant, returnedValue),
//            Q_ARG(QVariant, msg));

//    qDebug() << "QML function returned:" << returnedValue.toString();

    QVariant returnedValue;
    QVariant parameterValue;

    qDebug() << "Method count: " + object->metaObject()->methodCount();

    for(int x = 0; x < object->metaObject()->methodCount() - 1; x++) {
        qDebug() << object->metaObject()->method(x).name();
    }

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

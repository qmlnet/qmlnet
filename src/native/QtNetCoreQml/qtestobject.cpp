#include "qtestobject.h"
#include <QDebug>
#include <QQmlEngine>

QML_DECLARE_TYPE(QAnotherTestObject)

QTestObject::QTestObject(QObject *parent) : QObject(parent), test(4)
{

}

QTestObject::~QTestObject()
{
    qDebug() << "Disposing QTestObject";
    QObject* parent = this->parent();
    if(parent) {
        qDebug() << parent->metaObject()->className();
    } else {
        qDebug() << "No parent";
    }
}

QTestObject* QTestObject::GetNewObject()
{
    QTestObject* o = new QTestObject(NULL);
    QQmlEngine::setObjectOwnership(o, QQmlEngine::JavaScriptOwnership);
    return o;
    //QQmlEngine::setObjectOwnership() with QQmlEngine::CppOwnership specified.
}

QVariant QTestObject::GetAnotherNewObject()
{
    QVariant variant;
    variant.setValue(new QAnotherTestObject(NULL));
    return variant;
//    QAnotherTestObject* o = new QAnotherTestObject(NULL);
//    QQmlEngine::setObjectOwnership(o, QQmlEngine::JavaScriptOwnership);
//    return o;
}

void QTestObject::DoSomething(QTestObject* o)
{
    qDebug() << o->Get();
}

QAnotherTestObject::QAnotherTestObject(QObject *parent) : QObject(parent), test(4)
{

}

QAnotherTestObject::~QAnotherTestObject()
{
    qDebug() << "Disposing QAnotherTestObject";
    QObject* parent = this->parent();
    if(parent) {
        qDebug() << parent->metaObject()->className();
    } else {
        qDebug() << "No parent";
    }
}

QAnotherTestObject* QAnotherTestObject::GetNewObject()
{
    QAnotherTestObject* o = new QAnotherTestObject(NULL);
    QQmlEngine::setObjectOwnership(o, QQmlEngine::JavaScriptOwnership);
    return o;
    //QQmlEngine::setObjectOwnership() with QQmlEngine::CppOwnership specified.
}

void QAnotherTestObject::DoSomething(QAnotherTestObject* o)
{
    qDebug() << o->Get2();
}

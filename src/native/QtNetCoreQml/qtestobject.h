#ifndef QTESTOBJECT_H
#define QTESTOBJECT_H

#include <QObject>
#include <QVariant>

class QAnotherTestObject;

class QTestObject : public QObject
{
    Q_OBJECT
public:
    explicit QTestObject(QObject *parent = nullptr);
    ~QTestObject();

    Q_INVOKABLE QTestObject* GetNewObject();

    Q_INVOKABLE QVariant GetAnotherNewObject();

    Q_INVOKABLE void DoSomething(QTestObject* o);

    int Get() {return test;}

signals:

public slots:
private:
    int test;
};

class QAnotherTestObject : public QObject
{
    Q_OBJECT
public:
    explicit QAnotherTestObject(QObject *parent = nullptr);
    ~QAnotherTestObject();

    Q_INVOKABLE QAnotherTestObject* GetNewObject();

    Q_INVOKABLE void DoSomething(QAnotherTestObject* o);

    int Get2() {return test;}

signals:

public slots:
private:
    int test;
};

#endif // QTESTOBJECT_H

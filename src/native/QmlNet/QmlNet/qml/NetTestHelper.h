#ifndef NETTESTHELPER_H
#define NETTESTHELPER_H

#include <QmlNet.h>
#include <QmlNet/qml/QQmlApplicationEngine.h>
#include <QDateTime>

class TestBaseQObject;
class TestQObject;
class TestDerivedQObject;

class TestBaseQObject : public QObject
{
    Q_OBJECT
public:
    Q_INVOKABLE TestBaseQObject();
    ~TestBaseQObject();
};

class TestQObject : public TestBaseQObject
{
    Q_OBJECT
    Q_PROPERTY(int readOnly READ getReadOnly)
    Q_PROPERTY(int readAndWrite READ getReadAndWrite WRITE setReadAndWrite)
    Q_PROPERTY(int propWithSignal READ getPropWithSignal WRITE setPropWithSignal NOTIFY propWithSignalChanged)
    Q_PROPERTY(QVariant variantProperty READ getVariantProperty WRITE setVariantProperty)
public:
    Q_INVOKABLE TestQObject();
    ~TestQObject();
    int getReadOnly();
    void setWriteOnly(int value);
    int getReadAndWrite();
    void setReadAndWrite(int value);
    int getPropWithSignal();
    void setPropWithSignal(int value);
    QVariant getVariantProperty();
    void setVariantProperty(QVariant value);

signals:
    void propWithSignalChanged(int value);
    void testSignal();
    void testSignalWithArg(int arg);
    void testSignalBool(bool value);
    void testSignalChar(QChar value);
    void testSignalInt(int value);
    void testSignalUInt(uint value);
    void testSignalLong(long value);
    void testSignalULong(ulong value);
    void testSignalFloat(float value);
    void testSignalDouble(double value);
    void testSignalString(QString value);
    void testSignalDateTime(QDateTime value);
    void testSignalQObject(QObject* qObject);
    void testSignalTypedBaseQObject(TestBaseQObject* value);
    void testSignalTypedQObject(TestQObject* value);
    void testSignalTypedDerivedQObject(TestDerivedQObject* value);
    void testSignalQInt32(qint32 value);
    void testSignalQUInt32(quint32 value);
    void testSignalQInt64(qint64 value);
    void testSignalQUInt64(quint64 value);
    void testSignalQVariantList(QVariantList value);

public slots:
    void testSlot();
    void testSlotWithArg(int arg);
    QVariant testVariantReturn();
    bool testSlotBool(bool value);
    QChar testSlotChar(QChar value);
    int testSlotInt(int value);
    uint testSlotUInt(uint value);
    long testSlotLong(long value);
    ulong testSlotULong(ulong value);
    float testSlotFloat(float value);
    double testSlotDouble(double value);
    QString testSlotString(QString value);
    QDateTime testSlotDateTime(QDateTime value);
    QObject* testSlotQObject(QObject* value);
    TestBaseQObject* testSlotTypedBaseQObject(TestBaseQObject* value);
    TestQObject* testSlotTypedQObject(TestQObject* value);
    TestDerivedQObject* testSlotTypedDerivedQObject(TestDerivedQObject* value);
    qint32 testSlotQInt32(qint32 value);
    quint32 testSlotQUInt32(quint32 value);
    qint64 testSlotQInt64(qint64 value);
    quint64 testSlotQUInt64(quint64 value);
    QVariantList testSlotQVariantList(QVariantList variantList);

private:
    int _writeOnly;
    int _readAndWrite;
    int _propWithSignal;
    QVariant _variantValue;
};

class TestDerivedQObject : public TestQObject
{
    Q_OBJECT
public:
    Q_INVOKABLE TestDerivedQObject();
    ~TestDerivedQObject();
private:

};

Q_DECLARE_METATYPE(TestBaseQObject*);
Q_DECLARE_METATYPE(TestQObject*);
Q_DECLARE_METATYPE(TestDerivedQObject*);

#endif // NETTESTHELPER_H

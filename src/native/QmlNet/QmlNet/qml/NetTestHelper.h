#ifndef NETTESTHELPER_H
#define NETTESTHELPER_H

#include <QmlNet.h>
#include <QmlNet/qml/QQmlApplicationEngine.h>
#include <QDateTime>

class TestQObject : public QObject
{
    Q_OBJECT
    Q_PROPERTY(int readOnly READ getReadOnly)
    Q_PROPERTY(int writeOnly WRITE setWriteOnly)
    Q_PROPERTY(int readAndWrite READ getReadAndWrite WRITE setReadAndWrite)
    Q_PROPERTY(int propWithSignal READ getPropWithSignal WRITE setPropWithSignal NOTIFY propWithSignalChanged)
    Q_PROPERTY(QVariant variantProperty READ getVariantProperty WRITE setVariantProperty)
public:
    TestQObject();
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

#endif // NETTESTHELPER_H

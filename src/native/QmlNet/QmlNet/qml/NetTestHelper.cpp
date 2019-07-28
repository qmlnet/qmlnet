#include <QmlNet/qml/NetTestHelper.h>
#include <QQmlComponent>
#include <QDebug>
#include <QCoreApplication>
#include <QQmlContext>

TestBaseQObject::TestBaseQObject()
    : QObject(nullptr)
{

}

TestBaseQObject::~TestBaseQObject()
{

}

TestQObject::TestQObject()
    : _readAndWrite(0),
    _propWithSignal(0)
{
}

TestQObject::~TestQObject()
{

}

int TestQObject::getReadOnly()
{
    return 3;
}

int TestQObject::getReadAndWrite()
{
    return _readAndWrite;
}

void TestQObject::setReadAndWrite(int value)
{
    _readAndWrite = value;
}

int TestQObject::getPropWithSignal()
{
    return _propWithSignal;
}

void TestQObject::setPropWithSignal(int value)
{
    if(value == _propWithSignal) {
        return;
    }
    _propWithSignal = value;
    emit propWithSignalChanged(value);
}

QVariant TestQObject::getVariantProperty()
{
    return _variantValue;
}

void TestQObject::setVariantProperty(QVariant value)
{
    _variantValue = value;
}

void TestQObject::testSlot()
{
    emit testSignal();
}

void TestQObject::testSlotWithArg(int arg)
{
    emit testSignalWithArg(arg);
}

QVariant TestQObject::testVariantReturn()
{
    return getVariantProperty();
}

bool TestQObject::testSlotBool(bool value)
{
    emit testSignalBool(value);
    return value;
}

QChar TestQObject::testSlotChar(QChar value)
{
    emit testSignalChar(value);
    return value;
}

int TestQObject::testSlotInt(int value)
{
    emit testSignalInt(value);
    return value;
}

uint TestQObject::testSlotUInt(uint value)
{
    emit testSignalUInt(value);
    return value;
}

long TestQObject::testSlotLong(long value)
{
    emit testSignalLong(value);
    return value;
}

ulong TestQObject::testSlotULong(ulong value)
{
    emit testSignalULong(value);
    return value;
}

float TestQObject::testSlotFloat(float value)
{
    emit testSignalFloat(value);
    return value;
}

double TestQObject::testSlotDouble(double value)
{
    emit testSignalDouble(value);
    return value;
}

QString TestQObject::testSlotString(QString value)
{
    emit testSignalString(value);
    return value;
}

QDateTime TestQObject::testSlotDateTime(QDateTime value)
{
    emit testSignalDateTime(value);
    return value;
}

QObject* TestQObject::testSlotQObject(QObject* value)
{
    emit testSignalQObject(value);
    return value;
}

TestBaseQObject* TestQObject::testSlotTypedBaseQObject(TestBaseQObject* value)
{
    emit testSignalTypedBaseQObject(value);
    return value;
}

TestQObject* TestQObject::testSlotTypedQObject(TestQObject* value)
{
    emit testSignalTypedQObject(value);
    return value;
}

TestDerivedQObject* TestQObject::testSlotTypedDerivedQObject(TestDerivedQObject* value)
{
    emit testSignalTypedDerivedQObject(value);
    return value;
}

qint32 TestQObject::testSlotQInt32(qint32 value)
{
    emit testSignalQInt32(value);
    return value;
}

quint32 TestQObject::testSlotQUInt32(quint32 value)
{
    emit testSignalQUInt32(value);
    return value;
}

qint64 TestQObject::testSlotQInt64(qint64 value)
{
    emit testSignalQInt64(value);
    return value;
}

quint64 TestQObject::testSlotQUInt64(quint64 value)
{
    emit testSignalQUInt64(value);
    return value;
}

QVariantList TestQObject::testSlotQVariantList(QVariantList variantList)
{
    emit testSignalQVariantList(variantList);
    return variantList;
}

TestDerivedQObject::TestDerivedQObject()
{
}

TestDerivedQObject::~TestDerivedQObject()
{

}

TestAssertions::TestAssertions() : failedAssertions(false)
{

}

TestAssertions::~TestAssertions()
{

}

void TestAssertions::isTrue(QJSValue del)
{
    if(del.isBool()) {
        if(del.toBool() != true) {
            failedAssertions = true;
        }

        return;
    }

    failedAssertions = true;
    qWarning() << "Unknown type to assert.";
}

void TestAssertions::isFalse(QJSValue del)
{
    if(del.isBool()) {
        if(del.toBool() != false) {
            failedAssertions = true;
        }

        return;
    }

    failedAssertions = true;
    qWarning() << "Unknown type to assert.";
}

extern "C" {

Q_DECL_EXPORT uchar net_test_helper_runQml(QQmlApplicationEngineContainer* qmlEngineContainer, LPWSTR qml, uchar runEvents)
{
    qRegisterMetaType<TestBaseQObject*>();
    qRegisterMetaType<TestQObject*>();
    qRegisterMetaType<TestDerivedQObject*>();
    qRegisterMetaType<TestAssertions*>();

    QQmlComponent component(qmlEngineContainer->qmlEngine);
    QString qmlString = QString::fromUtf16(static_cast<const char16_t*>(qml));
    component.setData(qmlString.toUtf8(), QUrl());

    QObject *object = component.create();

    if(object == nullptr) {
        qWarning() << "Couldn't create qml object.";
        return 0;
    }

    QSharedPointer<TestAssertions> testAssertions = QSharedPointer<TestAssertions>(new TestAssertions());
    QSharedPointer<TestQObject> testQObject = QSharedPointer<TestQObject>(new TestQObject());
    object->setProperty("testQObject", QVariant::fromValue(testQObject.data()));
    object->setProperty("assert", QVariant::fromValue(testAssertions.data()));
    QMetaObject::invokeMethod(object, "runTest");

    if(runEvents == 1) {
        QCoreApplication::sendPostedEvents(nullptr, QEvent::DeferredDelete);
        QCoreApplication::processEvents(QEventLoop::AllEvents);
    }

    delete object;

    if(testAssertions->failedAssertions) {
        qWarning() << "Failed assertions detected.";
        return 0;
    }

    return 1;
}

}

#include <QmlNet/qml/NetTestHelper.h>
#include <QQmlComponent>
#include <QDebug>
#include <QCoreApplication>

TestBaseQObject::TestBaseQObject()
    : QObject(nullptr)
{

}

TestBaseQObject::~TestBaseQObject()
{

}

TestQObject::TestQObject()
    : _writeOnly(0),
    _readAndWrite(0),
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

void TestQObject::setWriteOnly(int value)
{
    _writeOnly = value;
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

extern "C" {

using WarningCallback = void(const QChar*);

Q_DECL_EXPORT uchar net_test_helper_runQml(QQmlApplicationEngineContainer* qmlEngineContainer, QChar* qml, uchar runEvents, WarningCallback *warningCallback)
{
    qRegisterMetaType<TestBaseQObject*>();
    qRegisterMetaType<TestQObject*>();
    qRegisterMetaType<TestDerivedQObject*>();

    // Temporarily connect to QQmlEngine::warnings
    auto connection = QObject::connect(qmlEngineContainer->qmlEngine, &QQmlEngine::warnings, [=](const QList<QQmlError> &warnings) {
        for (const auto &qmlError : warnings) {
            warningCallback(qmlError.toString().data());
        }
    });

    QQmlComponent component(qmlEngineContainer->qmlEngine);
    QString qmlString(qml);
    component.setData(qmlString.toUtf8(), QUrl());

    QObject *object = component.create();

    if(object == nullptr) {
        QObject::disconnect(connection);
        qWarning() << "Couldn't create qml object.";
        return 0;
    }

    QSharedPointer<TestQObject> testQObject = QSharedPointer<TestQObject>(new TestQObject());
    object->setProperty("testQObject", QVariant::fromValue(testQObject.data()));
    QMetaObject::invokeMethod(object, "runTest");

    if(runEvents == 1) {
        QCoreApplication::sendPostedEvents(nullptr, QEvent::DeferredDelete);
        QCoreApplication::processEvents(QEventLoop::AllEvents);
    }

    delete object;

    QObject::disconnect(connection);
    return 1;
}

}

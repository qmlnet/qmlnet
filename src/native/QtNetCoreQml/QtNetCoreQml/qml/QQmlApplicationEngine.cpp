#include <QtNetCoreQml/qml/QQmlApplicationEngine.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/qml/NetValueType.h>

#define DEFINE_NETVALUETYPE(N) \
    template<> QMetaObject NetValueType<N>::staticMetaObject = QMetaObject();

DEFINE_NETVALUETYPE(1)
DEFINE_NETVALUETYPE(2)
DEFINE_NETVALUETYPE(3)
DEFINE_NETVALUETYPE(4)
DEFINE_NETVALUETYPE(5)
DEFINE_NETVALUETYPE(6)
DEFINE_NETVALUETYPE(7)
DEFINE_NETVALUETYPE(8)
DEFINE_NETVALUETYPE(9)
DEFINE_NETVALUETYPE(10)
DEFINE_NETVALUETYPE(11)
DEFINE_NETVALUETYPE(12)
DEFINE_NETVALUETYPE(13)
DEFINE_NETVALUETYPE(14)
DEFINE_NETVALUETYPE(15)
DEFINE_NETVALUETYPE(16)
DEFINE_NETVALUETYPE(17)
DEFINE_NETVALUETYPE(18)
DEFINE_NETVALUETYPE(19)
DEFINE_NETVALUETYPE(20)
DEFINE_NETVALUETYPE(21)
DEFINE_NETVALUETYPE(22)
DEFINE_NETVALUETYPE(23)
DEFINE_NETVALUETYPE(24)
DEFINE_NETVALUETYPE(25)
DEFINE_NETVALUETYPE(26)
DEFINE_NETVALUETYPE(27)
DEFINE_NETVALUETYPE(28)
DEFINE_NETVALUETYPE(29)
DEFINE_NETVALUETYPE(30)

static int netValueTypeNumber = 0;

#define NETVALUETYPE_CASE(N) \
    case N: NetValueType<N>::init(typeInfo); return qmlRegisterType<NetValueType<N>>(uriString.toUtf8().data(), versionMajor, versionMinor, qmlNameString.toUtf8().data());

extern "C" {

QQmlApplicationEngineContainer* qqmlapplicationengine_create() {
    QQmlApplicationEngineContainer* result = new QQmlApplicationEngineContainer();
    result->qmlEngine = QSharedPointer<QQmlApplicationEngine>(new QQmlApplicationEngine());
    return result;
}

void qqmlapplicationengine_destroy(QQmlApplicationEngineContainer* container) {
    delete container;
}

void qqmlapplicationengine_load(QQmlApplicationEngineContainer* container, LPWSTR path) {
    container->qmlEngine->load(QString::fromUtf16(path));
}

int qqmlapplicationengine_registerType(NetTypeInfoContainer* typeContainer, LPWSTR uri, int versionMajor, int versionMinor, LPWSTR qmlName) {

    QString uriString = QString::fromUtf16(uri);
    QString qmlNameString = QString::fromUtf16(qmlName);
    QSharedPointer<NetTypeInfo> typeInfo = typeContainer->netTypeInfo;
    QString fullType = typeInfo->getFullTypeName();

    switch (++netValueTypeNumber) {
        NETVALUETYPE_CASE(1)
        NETVALUETYPE_CASE(2)
        NETVALUETYPE_CASE(3)
        NETVALUETYPE_CASE(4)
        NETVALUETYPE_CASE(5)
        NETVALUETYPE_CASE(6)
        NETVALUETYPE_CASE(7)
        NETVALUETYPE_CASE(8)
        NETVALUETYPE_CASE(9)
        NETVALUETYPE_CASE(10)
        NETVALUETYPE_CASE(11)
        NETVALUETYPE_CASE(12)
        NETVALUETYPE_CASE(13)
        NETVALUETYPE_CASE(14)
        NETVALUETYPE_CASE(15)
        NETVALUETYPE_CASE(16)
        NETVALUETYPE_CASE(17)
        NETVALUETYPE_CASE(18)
        NETVALUETYPE_CASE(19)
        NETVALUETYPE_CASE(20)
        NETVALUETYPE_CASE(21)
        NETVALUETYPE_CASE(22)
        NETVALUETYPE_CASE(23)
        NETVALUETYPE_CASE(24)
        NETVALUETYPE_CASE(25)
        NETVALUETYPE_CASE(26)
        NETVALUETYPE_CASE(27)
        NETVALUETYPE_CASE(28)
        NETVALUETYPE_CASE(29)
        NETVALUETYPE_CASE(30)
    }
    qFatal("Too many registered types");
    return -1;
}

}

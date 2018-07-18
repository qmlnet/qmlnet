#include <QtNetCoreQml/qml/QQmlApplicationEngine.h>
#include <QtNetCoreQml/types/NetTypeInfo.h>
#include <QtNetCoreQml/qml/NetValueType.h>

static int netValueTypeNumber = 0;

#define NETVALUETYPE_CASE(N) \
    case N: NetValueType<N>::init(typeInfo); return qmlRegisterType<NetValueType<N>>(uriString.toUtf8().data(), versionMajor, versionMinor, qmlNameString.toUtf8().data());

extern "C" {

Q_DECL_EXPORT QQmlApplicationEngineContainer* qqmlapplicationengine_create() {
    QQmlApplicationEngineContainer* result = new QQmlApplicationEngineContainer();
    result->qmlEngine = QSharedPointer<QQmlApplicationEngine>(new QQmlApplicationEngine());
    return result;
}

Q_DECL_EXPORT void qqmlapplicationengine_destroy(QQmlApplicationEngineContainer* container) {
    delete container;
}

Q_DECL_EXPORT void qqmlapplicationengine_load(QQmlApplicationEngineContainer* container, LPWSTR path) {
    container->qmlEngine->load(QString::fromUtf16((const char16_t*)path));
}

Q_DECL_EXPORT int qqmlapplicationengine_registerType(NetTypeInfoContainer* typeContainer, LPWSTR uri, int versionMajor, int versionMinor, LPWSTR qmlName) {

    QString uriString = QString::fromUtf16((const char16_t*)uri);
    QString qmlNameString = QString::fromUtf16((const char16_t*)qmlName);
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
        NETVALUETYPE_CASE(31)
        NETVALUETYPE_CASE(32)
        NETVALUETYPE_CASE(33)
        NETVALUETYPE_CASE(34)
        NETVALUETYPE_CASE(35)
        NETVALUETYPE_CASE(36)
        NETVALUETYPE_CASE(37)
        NETVALUETYPE_CASE(38)
        NETVALUETYPE_CASE(39)
        NETVALUETYPE_CASE(40)
        NETVALUETYPE_CASE(41)
        NETVALUETYPE_CASE(42)
        NETVALUETYPE_CASE(43)
        NETVALUETYPE_CASE(44)
        NETVALUETYPE_CASE(45)
        NETVALUETYPE_CASE(46)
        NETVALUETYPE_CASE(47)
        NETVALUETYPE_CASE(48)
        NETVALUETYPE_CASE(49)
        NETVALUETYPE_CASE(50)
    }
    qFatal("Too many registered types");
    return -1;
}

}

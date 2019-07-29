#include "NetAotMethodInvocation.h"
#include <QQmlApplicationEngine>
#include <QmlNet/types/Callbacks.h>
NetAotMethodInvocation::NetAotMethodInvocation() : NetObject(false)
{
	_netReference = QmlNet::instantiateType(nullptr, 1);
}
NetAotMethodInvocation::NetAotMethodInvocation(bool) : NetObject(false)
{
}
int NetAotMethodInvocation::registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName)
{
	return qmlRegisterType<NetAotMethodInvocation>(uri, versionMajor, versionMinor, qmlName);
}
int NetAotMethodInvocation::registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName)
{
	return qmlRegisterSingletonType<NetAotMethodInvocation>(uri, versionMajor, versionMinor, typeName, NetAotMethodInvocation::build);
}
QObject* NetAotMethodInvocation::build(QQmlEngine *engine, QJSEngine *scriptEngine)
{
	Q_UNUSED(engine)
	Q_UNUSED(scriptEngine)
	return new NetAotMethodInvocation();
}

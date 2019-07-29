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
int NetAotMethodInvocation::_registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName)
{
	return qmlRegisterType<NetAotMethodInvocation>(uri, versionMajor, versionMinor, qmlName);
}
int NetAotMethodInvocation::_registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName)
{
	return qmlRegisterSingletonType<NetAotMethodInvocation>(uri, versionMajor, versionMinor, typeName, NetAotMethodInvocation::_build);
}
QObject* NetAotMethodInvocation::_build(QQmlEngine *engine, QJSEngine *scriptEngine)
{
	Q_UNUSED(engine)
	Q_UNUSED(scriptEngine)
	return new NetAotMethodInvocation();
}
void NetAotMethodInvocation::method1()
{
}
void NetAotMethodInvocation::method2()
{
}

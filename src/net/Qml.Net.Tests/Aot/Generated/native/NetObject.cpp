#include "NetObject.h"
#include <QQmlApplicationEngine>
#include <QmlNet/types/Callbacks.h>
NetObject::NetObject() : QObject()
{
	_netReference = QmlNet::instantiateType(nullptr, 2);
}
NetObject::NetObject(bool) : QObject()
{
}
int NetObject::_registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName)
{
	return qmlRegisterType<NetObject>(uri, versionMajor, versionMinor, qmlName);
}
int NetObject::_registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName)
{
	return qmlRegisterSingletonType<NetObject>(uri, versionMajor, versionMinor, typeName, NetObject::_build);
}
QObject* NetObject::_build(QQmlEngine *engine, QJSEngine *scriptEngine)
{
	Q_UNUSED(engine)
	Q_UNUSED(scriptEngine)
	return new NetObject();
}

#ifndef NETAOTMETHODINVOCATION_H
#define NETAOTMETHODINVOCATION_H
#include "NetObject.h"
class QQmlEngine;
class QJSEngine;
class NetAotMethodInvocation : public NetObject
{
	Q_OBJECT
public:
	NetAotMethodInvocation();
	NetAotMethodInvocation(bool);
	static int _registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName);
	static int _registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName);
	static QObject* _build(QQmlEngine* engine, QJSEngine* scriptEngine);
	Q_INVOKABLE void method1();
	Q_INVOKABLE void method2();
};
#endif // NETAOTMETHODINVOCATION_H

#ifndef NETAOTMETHODINVOCATION_H
#define NETAOTMETHODINVOCATION_H
#include "NetObject.h"
class QQmlEngine;
class QJSEngine;
class NetAotMethodInvocation : public NetObject
{
public:
	NetAotMethodInvocation();
	NetAotMethodInvocation(bool);
	static int registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName);
	static int registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName);
	static QObject *build(QQmlEngine *engine, QJSEngine *scriptEngine);
};
#endif // NETAOTMETHODINVOCATION_H

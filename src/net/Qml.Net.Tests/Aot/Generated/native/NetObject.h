#ifndef NETOBJECT_H
#define NETOBJECT_H
#include <QObject>
#include <QmlNet/types/NetReference.h>
class QQmlEngine;
class QJSEngine;
class NetObject: public QObject
{
public:
	NetObject();
	NetObject(bool);
	static int registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName);
	static int registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName);
	static QObject *build(QQmlEngine *engine, QJSEngine *scriptEngine);
protected:
	QSharedPointer<NetReference> _netReference;
};
#endif // NETOBJECT_H

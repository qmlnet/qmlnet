#ifndef NETOBJECT_H
#define NETOBJECT_H
#include <QObject>
#include <QmlNet/types/NetReference.h>
class QQmlEngine;
class QJSEngine;
class NetObject: public QObject
{
	Q_OBJECT
public:
	NetObject();
	NetObject(bool);
	static int _registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName);
	static int _registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName);
	static QObject* _build(QQmlEngine* engine, QJSEngine* scriptEngine);
protected:
	QSharedPointer<NetReference> _netReference;
};
#endif // NETOBJECT_H

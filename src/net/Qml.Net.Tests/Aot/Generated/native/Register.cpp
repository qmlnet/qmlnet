#include "Register.h"
#include <QCoreApplication>
#include <QMutex>
#include <QmlNet/types/NetTypeManager.h>
#include "NetAotMethodInvocation.h"
#include "NetObject.h"
static bool initAotTypesDone = false;
Q_GLOBAL_STATIC(QMutex, initAotTypesMutex);

Q_DECL_EXPORT void initAotTypes()
{
	initAotTypesMutex->lock();
	if(initAotTypesDone) {
		initAotTypesMutex->unlock();
		return;
	}
	NetTypeManager::registerAotObject(1, &NetAotMethodInvocation::staticMetaObject, NetAotMethodInvocation::_registerQml, NetAotMethodInvocation::_registerQmlSingleton);
	NetTypeManager::registerAotObject(2, &NetObject::staticMetaObject, NetObject::_registerQml, NetObject::_registerQmlSingleton);
	initAotTypesDone = true;
	initAotTypesMutex->unlock();
}

Q_COREAPP_STARTUP_FUNCTION(initAotTypes)

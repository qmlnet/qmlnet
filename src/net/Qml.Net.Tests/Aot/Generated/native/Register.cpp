#include "Register.h"
#include <QCoreApplication>
#include <QMutex>
#include <QmlNet/types/NetTypeManager.h>
#include "NetAotMethodInvocation.h"
#include "NetObject.h"
static bool initAotTypesDone = false;
Q_GLOBAL_STATIC(QMutex, initAotTypesMutex);

static void initAotTypes()
{
	initAotTypesMutex->lock();
	if(initAotTypesDone) {
		initAotTypesMutex->unlock();
		return;
	}
	NetTypeManager::registerAotObject(&NetAotMethodInvocation::staticMetaObject, 1);
	NetTypeManager::registerAotObject(&NetObject::staticMetaObject, 2);
	initAotTypesDone = true;
	initAotTypesMutex->unlock();
}

Q_COREAPP_STARTUP_FUNCTION(initAotTypes)

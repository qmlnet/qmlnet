#ifndef NET_QML_ACTIVATE_SIGNAL_H
#define NET_QML_ACTIVATE_SIGNAL_H

#include <QString>
#include "net_variant.h"

void activateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args);
bool tryActivateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args);

#endif // NET_QML_ACTIVATE_SIGNAL_H

%{
#include "net_qml_activate_signal.h"
%}

void activateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args);
bool tryActivateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args);
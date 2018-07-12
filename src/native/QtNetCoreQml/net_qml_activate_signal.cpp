#include "net_qml_activate_signal.h"

#include "net_type_info_manager.h"
#include <QDebug>

void activateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args) {
    NetVariant* result = nullptr;
    NetTypeInfo* typeInfo = NetTypeInfoManager::GetTypeInfo(const_cast<char*>(netType.c_str()));
    typeInfo->ActivateSignal(instance, signalName, args);
}

bool tryActivateSignal(NetGCHandle* instance, std::string netType, std::string signalName, std::vector<NetVariant*> args) {
    NetVariant* result = nullptr;
    NetTypeInfo* typeInfo = NetTypeInfoManager::GetTypeInfo(const_cast<char*>(netType.c_str()));
    return typeInfo->TryActivateSignal(instance, signalName, args);
}

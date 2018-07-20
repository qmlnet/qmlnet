#include <QtNetCoreQml/types/NetSignalInfo.h>
#include <iostream>

NetSignalInfo::NetSignalInfo(QString name) :
    _name(name) {
}

QString NetSignalInfo::getName() {
    return _name;
}

void NetSignalInfo::addParameter(NetVariantTypeEnum type) {
    if(type == NetVariantTypeEnum_Invalid) return;
    _parameters.append(type);
}

uint NetSignalInfo::getParameterCount() {
    return _parameters.size();
}

NetVariantTypeEnum NetSignalInfo::getParameter(uint index) {
    if(index >= (uint)_parameters.length()) return NetVariantTypeEnum_Invalid;
    return _parameters.at(index);
}

extern "C" {

Q_DECL_EXPORT NetSignalInfoContainer* signal_info_create(LPWSTR name) {
    NetSignalInfoContainer* result = new NetSignalInfoContainer();
    NetSignalInfo* instance = new NetSignalInfo(QString::fromUtf16((const char16_t*)name));
    result->signal = QSharedPointer<NetSignalInfo>(instance);
    return result;
}

Q_DECL_EXPORT void signal_info_destroy(NetSignalInfoContainer* container) {
    delete container;
}

Q_DECL_EXPORT LPWSTR signal_info_getName(NetSignalInfoContainer* container) {
    return (LPWSTR)container->signal->getName().utf16();
}

Q_DECL_EXPORT void signal_info_addParameter(NetSignalInfoContainer* container, NetVariantTypeEnum type) {
    container->signal->addParameter(type);
}

Q_DECL_EXPORT uint signal_info_getParameterCount(NetSignalInfoContainer* container) {
    return container->signal->getParameterCount();
}

Q_DECL_EXPORT NetVariantTypeEnum signal_info_getParameter(NetSignalInfoContainer* container, uint index) {
    return container->signal->getParameter(index);
}

}

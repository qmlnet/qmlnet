#include <QtNetCoreQml/types/net_type_info_manager.h>
#include <iostream>

static NetTypeInfoManagerCallbacks* sharedCallbacks = nullptr;

void registerCallbacks() {
    std::cout << "test" << std::endl;
}

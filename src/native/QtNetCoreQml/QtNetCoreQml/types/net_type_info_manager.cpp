#include <QtNetCoreQml/types/net_type_info_manager.h>
#include <iostream>

static NetTypeInfoManagerCallbacks sharedCallbacks;

void type_info_manager_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

bool type_info_manager_isTypeValid(LPSTR typeName) {
    return sharedCallbacks.isTypeValid(typeName);
}

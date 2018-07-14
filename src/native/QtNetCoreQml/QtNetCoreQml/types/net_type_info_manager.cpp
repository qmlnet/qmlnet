#include <QtNetCoreQml/types/net_type_info_manager.h>
#include <iostream>

static NetTypeInfoManagerCallbacks sharedCallbacks;

void registerCallbacks(NetTypeInfoManagerCallbacks* callbacks) {
    sharedCallbacks = *callbacks;
}

bool isTypeValid() {
    sharedCallbacks.isTypeValid();
}

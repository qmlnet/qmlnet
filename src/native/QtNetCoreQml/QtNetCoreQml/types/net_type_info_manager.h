#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef bool (*isTypeValidCb)();

struct NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
};

void registerCallbacks(NetTypeInfoManagerCallbacks* callbacks);

bool isTypeValid();

#ifdef __cplusplus
}
#endif

#endif // NET_TYPE_INFO_MANAGER_H

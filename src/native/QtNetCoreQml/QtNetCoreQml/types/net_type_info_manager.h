#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef bool (*isTypeValid)();

struct NetTypeInfoManagerCallbacks {
    isTypeValid isTypeValid;
};

void registerCallbacks();

#ifdef __cplusplus
}
#endif

#endif // NET_TYPE_INFO_MANAGER_H

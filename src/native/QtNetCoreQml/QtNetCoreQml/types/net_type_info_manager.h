#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef bool (*isTypeValidCb)(LPWSTR typeName);

struct NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
};

void type_info_manager_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks);

bool type_info_manager_isTypeValid(LPWSTR typeName);

#ifdef __cplusplus
}
#endif

#endif // NET_TYPE_INFO_MANAGER_H

#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QtNetCoreQml.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef bool (*isTypeValidCb)(LPSTR typeName);

struct NetTypeInfoManagerCallbacks {
    isTypeValidCb isTypeValid;
};

void type_info_manager_registerCallbacks(NetTypeInfoManagerCallbacks* callbacks);

bool type_info_manager_isTypeValid(LPSTR typeName);

#ifdef __cplusplus
}
#endif

#endif // NET_TYPE_INFO_MANAGER_H

#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <qglobal.h>

class NetTypeInfo {
public:
    NetTypeInfo();
    ~NetTypeInfo();
};

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
    virtual bool isValidType(char* typeName) {
        Q_UNUSED(typeName);
        return false;
    }
};

class NetTypeInfoManager {
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
private:
    static NetTypeInfoCallbacks* callbacks;
};

#endif // NET_TYPE_INFO_H

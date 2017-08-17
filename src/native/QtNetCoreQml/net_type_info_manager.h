#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include "qtnetcoreqml_global.h"
#include <vector>
#include <QMap>

class NetTypeInfo;
class NetInstance;
class NetVariant;
class NetPropertyInfo;
class NetMethodInfo;

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
    virtual bool isValidType(char* typeName) {
        Q_UNUSED(typeName);
        return false;
    }
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo) {
        Q_UNUSED(typeInfo);
    }
    virtual void CreateInstance(NetTypeInfo* typeInfo, NetGCHandle** instance) {
        Q_UNUSED(typeInfo);
        Q_UNUSED(instance);
    }
    virtual void ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetVariant* result) {
        Q_UNUSED(propertyInfo);
        Q_UNUSED(target);
        Q_UNUSED(result);
    }
    virtual void WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetVariant* value) {
        Q_UNUSED(propertyInfo);
        Q_UNUSED(target);
        Q_UNUSED(value);
    }
    virtual void InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetVariant*> parameters, NetVariant* result) {
        Q_UNUSED(methodInfo);
        Q_UNUSED(target);
        Q_UNUSED(parameters);
        Q_UNUSED(result);
    }
    virtual void ReleaseGCHandle(NetGCHandle* gcHandle) {
        Q_UNUSED(gcHandle);
    }
    virtual void CopyGCHandle(NetGCHandle* gcHandle, NetGCHandle** gcHandleCopy) {
        Q_UNUSED(gcHandle);
        Q_UNUSED(gcHandleCopy);
    }
};

class NetTypeInfoManager {
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName, NetTypeInfo* returnType);
    static NetPropertyInfo* NewPropertyInfo(NetTypeInfo* parentTypeInfo, std::string propertyName, NetTypeInfo* returnType, bool canRead, bool canWrite);
    static NetInstance* CreateInstance(NetTypeInfo* typeInfo);
    static NetInstance* WrapCreatedInstance(NetGCHandle* gcHandle, NetTypeInfo* typeInfo);
    static NetVariant* ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target);
    static void WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetVariant* value);
    static NetVariant* InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetVariant*> parameters);
    static void ReleaseGCHandle(NetGCHandle* gcHandle);
    static NetGCHandle* CopyGCHandle(NetGCHandle* gcHandle);
private:
    static NetTypeInfoCallbacks* callbacks;
    static QMap<QString, NetTypeInfo*> types;
};


#endif // NET_TYPE_INFO_MANAGER_H

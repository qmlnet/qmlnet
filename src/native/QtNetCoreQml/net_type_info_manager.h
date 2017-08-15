#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include "qtnetcoreqml_global.h"
#include <vector>
#include <QMap>

class NetTypeInfo;
class NetInstance;
class NetPropertyInfo;
class NetMethodInfo;

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
    virtual bool isValidType(char* typeName) {
        Q_UNUSED(typeName);
        return false;
    }
    virtual NetInterTypeEnum GetNetInterType(char* typeName) {
        Q_UNUSED(typeName);
        return NetInterTypeEnum_Object;
    }
    virtual void BuildTypeInfo(NetTypeInfo* typeInfo) {
        Q_UNUSED(typeInfo);
    }
    virtual void CreateInstance(NetTypeInfo* typeInfo, NetInstance* instance) {
        Q_UNUSED(typeInfo);
        Q_UNUSED(instance);
    }
    virtual void ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetInstance* result) {
        Q_UNUSED(propertyInfo);
        Q_UNUSED(target);
        Q_UNUSED(result);
    }
    virtual void WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetInstance* value) {
        Q_UNUSED(propertyInfo);
        Q_UNUSED(target);
        Q_UNUSED(value);
    }
    virtual void InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetInstance*> parameters, NetInstance* result) {
        Q_UNUSED(methodInfo);
        Q_UNUSED(target);
        Q_UNUSED(parameters);
        Q_UNUSED(result);
    }
};

class NetTypeInfoManager {
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetInterTypeEnum GetNetInterType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName, NetTypeInfo* returnType);
    static NetPropertyInfo* NewPropertyInfo(NetTypeInfo* parentTypeInfo, std::string propertyName, NetTypeInfo* returnType, bool canRead, bool canWrite);
    static NetInstance* CreateInstance(NetTypeInfo* typeInfo);
    static NetInstance* ReadProperty(NetPropertyInfo* propertyInfo, NetInstance* target);
    static void WriteProperty(NetPropertyInfo* propertyInfo, NetInstance* target, NetInstance* value);
    static NetInstance* InvokeMethod(NetMethodInfo* methodInfo, NetInstance* target, std::vector<NetInstance*> parameters);
private:
    static NetTypeInfoCallbacks* callbacks;
    static QMap<QString, NetTypeInfo*> types;
};


#endif // NET_TYPE_INFO_MANAGER_H

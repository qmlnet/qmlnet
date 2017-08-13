#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include "qtnetcoreqml_global.h"
#include "net_instance.h"
#include <qglobal.h>
#include <QMap>
#include <QDebug>

class NetTypeInfo;
class NetMethodInfo;
class QMetaObject;

struct NetTypeInfoParameter
{
    std::string name;
    NetTypeInfo *typeInfo;
};

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName);
    ~NetTypeInfo();
    std::string GetTypeName();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    QMetaObject* metaObject;
private:
    std::string typeName;
    QList<NetMethodInfo*> methods;
};

class NetMethodInfo {
public:
    NetMethodInfo(NetTypeInfo *parentTypeInfo, std::string methodName);
    std::string GetMethodName();
    void SetReturnType(NetTypeInfo *typeInfo);
    NetTypeInfo* GetReturnType();
    void AddParameter(std::string parameterName, NetTypeInfo *typeInfo);
    int GetParameterCount();
    void GetParameterInfo(int index, std::string *parameterName, NetTypeInfo **typeInfo);
private:
    NetTypeInfo *parentTypeInfo;
    std::string methodName;
    NetTypeInfo *returnType;
    QList<NetTypeInfoParameter> parameters;
};

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
    virtual void CreateInstance(char* typeName, NetInstance* instance) {
        Q_UNUSED(typeName);
        Q_UNUSED(instance);
    }
};

class NetTypeInfoManager {
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName);
    static NetInstance* CreateInstance(NetTypeInfo* typeInfo);
private:
    static NetTypeInfoCallbacks* callbacks;
    static QMap<QString, NetTypeInfo*> types;
};


#endif // NET_TYPE_INFO_H

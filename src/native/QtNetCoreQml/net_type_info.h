#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <qglobal.h>
#include <QMap>
#include <QDebug>

class NetTypeInfo;
class NetMethodInfo;

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
private:
    std::string typeName;
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
        qDebug() << "FFF";
        Q_UNUSED(typeInfo);
    }
};

class NetTypeInfoManager {
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
    static NetTypeInfo* GetTypeInfo(char* typeName);
    static NetMethodInfo* NewMethodInfo(NetTypeInfo* parentTypeInfo, char* methodName);
private:
    static NetTypeInfoCallbacks* callbacks;
    static QMap<QString, NetTypeInfo*> types;
};


#endif // NET_TYPE_INFO_H

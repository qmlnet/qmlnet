#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <qglobal.h>
#include <QMap>
#include <QDebug>

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName);
    ~NetTypeInfo();
    std::string GetTypeName();
private:
    std::string typeName;
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
private:
    static NetTypeInfoCallbacks* callbacks;
    static QMap<QString, NetTypeInfo*> types;
};


#endif // NET_TYPE_INFO_H

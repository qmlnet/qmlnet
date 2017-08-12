#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <string>
#include <QDebug>

class NetTypeInfoCallbacks {
public:
    virtual ~NetTypeInfoCallbacks() { }
    virtual bool isValidType(char* typeName) {
        qDebug() << "sdfsdfsfd";
        return false;
    }
};

class NetTypeInfoManager
{
public:
    NetTypeInfoManager();
    static void setCallbacks(NetTypeInfoCallbacks* callbacks);
    static bool isValidType(char* typeName);
private:
    static NetTypeInfoCallbacks* callbacks;
};

#endif // NET_TYPE_INFO_H

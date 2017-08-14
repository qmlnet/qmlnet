#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include "qtnetcoreqml_global.h"
#include <QMetaObject>
#include <QList>

class NetTypeInfo;
class NetMethodInfo;
class NetPropertyInfo;

class NetTypeInfo {
public:
    NetTypeInfo(std::string typeName, NetInterTypeEnum interType);
    ~NetTypeInfo();
    std::string GetTypeName();
    NetInterTypeEnum GetInterType();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    void AddProperty(NetPropertyInfo* propertyInfo);
    int GetPropertyCount();
    NetPropertyInfo* GetProperty(int index);
    QMetaObject* metaObject;
private:
    std::string typeName;
    QList<NetMethodInfo*> methods;
    QList<NetPropertyInfo*> properties;
    NetInterTypeEnum interType;
};


#endif // NET_TYPE_INFO_H

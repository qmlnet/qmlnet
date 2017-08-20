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
    NetTypeInfo(std::string typeName);
    ~NetTypeInfo();
    NetVariantTypeEnum GetPrefVariantType();
    void SetPrefVariantType(NetVariantTypeEnum value);
    std::string GetTypeName();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    void AddProperty(NetPropertyInfo* propertyInfo);
    int GetPropertyCount();
    NetPropertyInfo* GetProperty(int index);
    QMetaObject* metaObject;
private:
    NetVariantTypeEnum prefVariantType;
    std::string typeName;
    QList<NetMethodInfo*> methods;
    QList<NetPropertyInfo*> properties;
};


#endif // NET_TYPE_INFO_H

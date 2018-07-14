#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QtNetCoreQml.h>
#include <QList>

class NetMethodInfo;
class NetPropertyInfo;
class NetValue;

class NetTypeInfo {
public:
    NetTypeInfo(std::string fullTypeName);
    ~NetTypeInfo();
    NetVariantTypeEnum GetPrefVariantType();
    void SetPrefVariantType(NetVariantTypeEnum value);
    std::string GetFullTypeName();
    void SetClassName(std::string className);
    std::string GetClassName();
    void AddMethod(NetMethodInfo* methodInfo);
    int GetMethodCount();
    NetMethodInfo* GetMethod(int index);
    void AddProperty(NetPropertyInfo* propertyInfo);
    int GetPropertyCount();
    NetPropertyInfo* GetProperty(int index);
private:
    NetVariantTypeEnum prefVariantType;
    std::string fullTypeName;
    std::string className;
    QList<NetMethodInfo*> methods;
    QList<NetPropertyInfo*> properties;
};


#endif // NET_TYPE_INFO_H

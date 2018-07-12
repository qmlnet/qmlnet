#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include "qtnetcoreqml_global.h"
#include <QMetaObject>
#include <QList>
#include <QMap>
#include <vector>

class NetTypeInfo;
class NetMethodInfo;
class NetPropertyInfo;
class NetVariant;
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
    void RegisterNetInstance(NetGCHandle* instance, NetValue* qmlObject);
    void UnregisterNetInstance(NetValue* qmlObject);
    NetPropertyInfo* GetProperty(int index);
    void ActivateSignal(NetGCHandle* instance, std::string signalName, std::vector<NetVariant*> args);
    bool TryActivateSignal(NetGCHandle* instance, std::string signalName, std::vector<NetVariant*> args);
    QMetaObject* metaObject;
private:
    NetVariantTypeEnum prefVariantType;
    std::string fullTypeName;
    std::string className;
    QList<NetMethodInfo*> methods;
    QList<NetPropertyInfo*> properties;
    QMap<NetGCHandle*, QList<NetValue*>> netHandleValuesMap;
    QMap<NetValue*, NetGCHandle*> netValuesHandleMap;
};


#endif // NET_TYPE_INFO_H

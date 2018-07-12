#include "net_type_info.h"
#include <QDebug>

#include "net_qml_value.h"
#include "net_instance.h"

NetTypeInfo::NetTypeInfo(std::string fullTypeName) :
    prefVariantType(NetVariantTypeEnum_Invalid),
    fullTypeName(fullTypeName),
    metaObject(NULL)
{
}

NetTypeInfo::~NetTypeInfo()
{

}

NetVariantTypeEnum NetTypeInfo::GetPrefVariantType()
{
    return prefVariantType;
}

void NetTypeInfo::SetPrefVariantType(NetVariantTypeEnum value)
{
    prefVariantType = value;
}

std::string NetTypeInfo::GetFullTypeName()
{
    return fullTypeName;
}

void NetTypeInfo::SetClassName(std::string className)
{
    this->className = className;
}

std::string NetTypeInfo::GetClassName()
{
    return className;
}

void NetTypeInfo::AddMethod(NetMethodInfo* methodInfo)
{
    methods.append(methodInfo);
}

int NetTypeInfo::GetMethodCount()
{
    return methods.length();
}

NetMethodInfo* NetTypeInfo::GetMethod(int index)
{
    if(index < 0) return NULL;
    if(index >= methods.length()) return NULL;

    return methods.at(index);
}

void NetTypeInfo::AddProperty(NetPropertyInfo* propertyInfo)
{
    properties.append(propertyInfo);
}

int NetTypeInfo::GetPropertyCount()
{
    return properties.length();
}

void NetTypeInfo::RegisterNetInstance(NetGCHandle* instance, NetValue* qmlObject)
{
    if(!netHandleValuesMap.contains(instance))
    {
        netHandleValuesMap.insert(instance, QList<NetValue*>());
    }
    if(!netHandleValuesMap[instance].contains(qmlObject))
    {
        netHandleValuesMap[instance].append(qmlObject);
    }

    if(!netValuesHandleMap.contains(qmlObject))
    {
        netValuesHandleMap.insert(qmlObject, instance);
    }
}

void NetTypeInfo::UnregisterNetInstance(NetValue* qmlObject)
{
    if(netValuesHandleMap.contains(qmlObject))
    {
        auto netInstance = netValuesHandleMap[qmlObject];
        netValuesHandleMap.remove(qmlObject);
        if(netHandleValuesMap.contains(netInstance))
        {
            netHandleValuesMap[netInstance].removeAll(qmlObject);
            if(netHandleValuesMap[netInstance].empty())
            {
                netHandleValuesMap.remove(netInstance);
            }
        }
    }
}

NetPropertyInfo* NetTypeInfo::GetProperty(int index)
{
    if(index < 0) return NULL;
    if(index >= properties.length()) return NULL;

    return properties.at(index);
}

bool NetTypeInfo::TryActivateSignal(NetGCHandle* instance, std::string signalName, std::vector<NetVariant*> args)
{
    try
    {
        ActivateSignal(instance, signalName, args);
        return true;
    }
    catch(std::exception&)
    {
        return false;
    }
}

void NetTypeInfo::ActivateSignal(NetGCHandle* instance, std::string signalName, std::vector<NetVariant*> args)
{
    if(netHandleValuesMap.contains(instance))
    {
        for(const auto& obj : netHandleValuesMap[instance])
        {
            obj->ActivateSignal(signalName, args);
        }
    }
    else
    {
        qDebug() << "No instance found! Instance: " << (uint64_t)instance;
        throw std::invalid_argument("Given instance not registered! Unable to send signal '" + signalName + "'");
    }
}

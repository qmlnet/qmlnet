#ifndef NET_TYPE_INFO_PROPERTY_H
#define NET_TYPE_INFO_PROPERTY_H

#include <string>

class NetTypeInfo;

class NetPropertyInfo {
public:
    NetPropertyInfo(NetTypeInfo* parentTypeInfo,
                    std::string propertyName,
                    NetTypeInfo* returnType,
                    bool canRead,
                    bool canWrite,
                    std::string notifySignalName);
    NetTypeInfo* GetParentType();
    std::string GetPropertyName();
    NetTypeInfo* GetReturnType();
    bool CanRead();
    bool CanWrite();
    std::string GetNotifySignalName();
private:
    NetTypeInfo* parentTypeInfo;
    std::string propertyName;
    NetTypeInfo* returnType;
    bool canRead;
    bool canWrite;
    std::string notifySignalName;
};


#endif // NET_TYPE_INFO_PROPERTY_H

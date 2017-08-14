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
                    bool canWrite);
    NetTypeInfo* GetParentType();
    std::string GetPropertyName();
    NetTypeInfo* GetReturnType();
    bool CanRead();
    bool CanWrite();
private:
    NetTypeInfo* parentTypeInfo;
    std::string propertyName;
    NetTypeInfo* returnType;
    bool canRead;
    bool canWrite;
};


#endif // NET_TYPE_INFO_PROPERTY_H

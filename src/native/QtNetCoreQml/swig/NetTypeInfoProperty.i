%module(directors="1") example
%{
#include "net_type_info_property.h"
%}

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
};
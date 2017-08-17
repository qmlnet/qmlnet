#include "net_variant.h"
#include "net_type_info_manager.h"
#include <QDebug>

struct NetInstanceContainer
{
    NetInstance* netInstance;
};

Q_DECLARE_METATYPE(NetInstanceContainer)

NetVariant::NetVariant()
{

}

NetVariant::~NetVariant()
{
    ClearNetInstance();
}

NetVariantTypeEnum NetVariant::GetVariantType()
{
    switch(variant.type()) {
    case QVariant::Invalid:
        return NetVariantTypeEnum_Invalid;
    case QVariant::Bool:
        return NetVariantTypeEnum_Bool;
    case QVariant::Int:
        return NetVariantTypeEnum_Int;
    case QVariant::Double:
        return NetVariantTypeEnum_Double;
    case QVariant::String:
        return NetVariantTypeEnum_String;
    case QVariant::Date:
        return NetVariantTypeEnum_Date;
    case QVariant::UserType:
        if(strcmp(variant.typeName(), "NetInstanceContainer") == 0)
            return NetVariantTypeEnum_Object;
        qDebug() << "Unknown user type for NetVariant: " << variant.typeName();
        return NetVariantTypeEnum_Invalid;
    default:
        qDebug() << "Unsupported qt variant type: " << variant.type();
        return NetVariantTypeEnum_Invalid;
    }
}

void NetVariant::SetNetInstance(NetInstance* netInstance)
{
    ClearNetInstance();
    variant.setValue(NetInstanceContainer{ netInstance });
}

NetInstance* NetVariant::GetNetInstance()
{
    qDebug() << variant.value<NetInstanceContainer>().netInstance->GetTypeInfo()->GetTypeName().c_str();
    return variant.value<NetInstanceContainer>().netInstance;
}

void NetVariant::SetBool(bool value)
{
    ClearNetInstance();
    variant.setValue(value);
}

bool NetVariant::GetBool()
{
    if(variant.canConvert(QMetaType::Bool))
        return variant.value<int>();

    qDebug() << "Can't convert value to bool";

    return false;
}

void NetVariant::SetInt(int value)
{
    ClearNetInstance();
    variant.setValue(value);
}

int NetVariant::GetInt()
{
    if(variant.canConvert(QMetaType::Int))
        return variant.value<int>();

    qDebug() << "Can't convert value to int";

    return 0;
}

void NetVariant::ClearNetInstance()
{
    if(variant.canConvert<NetInstanceContainer>())
    {
        delete variant.value<NetInstanceContainer>().netInstance;
        variant.clear();
    }
}

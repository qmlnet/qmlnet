#include "net_variant.h"
#include "net_type_info_manager.h"
#include <QDateTime>
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
    case QVariant::Char:
        return NetVariantTypeEnum_Char;
    case QVariant::Int:
        return NetVariantTypeEnum_Int;
    case QVariant::UInt:
        return NetVariantTypeEnum_UInt;
    case QVariant::Double:
        return NetVariantTypeEnum_Double;
    case QVariant::String:
        return NetVariantTypeEnum_String;
    case QVariant::DateTime:
        return NetVariantTypeEnum_DateTime;
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

void NetVariant::SetChar(QChar value)
{
    ClearNetInstance();
    variant.setValue(value);
}

QChar NetVariant::GetChar()
{
    return variant.toChar();
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

void NetVariant::SetUInt(unsigned int value)
{
    ClearNetInstance();
    variant.setValue(value);
}

unsigned int NetVariant::GetUInt()
{
    bool ok = false;
    unsigned int result = variant.toUInt(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to unsigned int";
    }

    return result;
}

void NetVariant::SetDouble(double value)
{
    ClearNetInstance();
    variant.setValue(value);
}

double NetVariant::GetDouble()
{
    bool ok = false;
    double result = variant.toDouble(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to double";
    }

    return result;
}

void NetVariant::SetString(QString* value)
{
    ClearNetInstance();
    if(value) {
        variant.setValue(*value);
    } else {
        variant.clear();
    }
}

QString NetVariant::GetString()
{
    if(variant.type() != QVariant::String) {
        qDebug() << "Variant is not a string";
        return "";
    }

    return variant.toString();
}

void NetVariant::SetDateTime(QDateTime& value)
{
    ClearNetInstance();
    if(value.isNull()) {
        variant.clear();
    } else {
        variant.setValue(value);
    }
}

QDateTime NetVariant::GetDateTime()
{
    return variant.toDateTime();
}

void NetVariant::Clear()
{
    ClearNetInstance();
    variant.clear();
}

QVariant NetVariant::AsQVariant()
{
    return variant;
}

void NetVariant::ClearNetInstance()
{
    if(variant.canConvert<NetInstanceContainer>())
    {
        delete variant.value<NetInstanceContainer>().netInstance;
        variant.clear();
    }
}

#include <QtNetCoreQml/qml/NetVariant.h>
#include <QDateTime>
#include <QDebug>

struct NetInstanceContainer
{
    QSharedPointer<NetInstance> netInstance;
};

Q_DECLARE_METATYPE(NetInstanceContainer)

NetVariant::NetVariant()
{

}

NetVariant::~NetVariant()
{
    clearNetInstance();
}

NetVariantTypeEnum NetVariant::getVariantType()
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
        qWarning() << "Unknown user type for NetVariant: " << variant.typeName();
        return NetVariantTypeEnum_Invalid;
    default:
        qWarning() << "Unsupported qt variant type: " << variant.type();
        return NetVariantTypeEnum_Invalid;
    }
}

void NetVariant::setNetInstance(QSharedPointer<NetInstance> netInstance)
{
    clearNetInstance();
    variant.setValue(NetInstanceContainer{ netInstance });
}

QSharedPointer<NetInstance> NetVariant::getNetInstance()
{
    return variant.value<NetInstanceContainer>().netInstance;
}

void NetVariant::setBool(bool value)
{
    clearNetInstance();
    variant.setValue(value);
}

bool NetVariant::getBool()
{
    if(variant.canConvert(QMetaType::Bool))
        return variant.value<int>();

    qDebug() << "Can't convert value to bool";

    return false;
}

void NetVariant::setChar(QChar value)
{
    clearNetInstance();
    variant.setValue(value);
}

QChar NetVariant::getChar()
{
    return variant.toChar();
}

void NetVariant::setInt(int value)
{
    clearNetInstance();
    variant.setValue(value);
}

int NetVariant::getInt()
{
    if(variant.canConvert(QMetaType::Int))
        return variant.value<int>();

    qDebug() << "Can't convert value to int";

    return 0;
}

void NetVariant::setUInt(unsigned int value)
{
    clearNetInstance();
    variant.setValue(value);
}

unsigned int NetVariant::getUInt()
{
    bool ok = false;
    unsigned int result = variant.toUInt(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to unsigned int";
    }

    return result;
}

void NetVariant::setDouble(double value)
{
    clearNetInstance();
    variant.setValue(value);
}

double NetVariant::getDouble()
{
    bool ok = false;
    double result = variant.toDouble(&ok);

    if(!ok) {
        qDebug() << "Couldn't convert variant to double";
    }

    return result;
}

void NetVariant::setString(QString* value)
{
    clearNetInstance();
    if(value) {
        variant.setValue(*value);
    } else {
        variant.clear();
    }
}

QString NetVariant::getString()
{
    if(variant.type() != QVariant::String) {
        qDebug() << "Variant is not a string";
        return "";
    }

    return variant.toString();
}

void NetVariant::setDateTime(QDateTime& value)
{
    clearNetInstance();
    if(value.isNull()) {
        variant.clear();
    } else {
        variant.setValue(value);
    }
}

QDateTime NetVariant::getDateTime()
{
    return variant.toDateTime();
}

void NetVariant::clear()
{
    clearNetInstance();
    variant.clear();
}

QVariant NetVariant::asQVariant()
{
    return variant;
}

void NetVariant::clearNetInstance()
{
    if(variant.canConvert<NetInstanceContainer>())
    {
        variant.value<NetInstanceContainer>().netInstance.clear();
        variant.clear();
    }
}

extern "C" {


NetVariantContainer* net_variant_create() {
    NetVariantContainer* result = new NetVariantContainer();
    result->variant = QSharedPointer<NetVariant>(new NetVariant());
    return result;
}

void net_variant_destroy(NetVariantContainer* container) {
    delete container;
}

NetVariantTypeEnum net_variant_getVariantType(NetVariantContainer* container) {
    return container->variant->getVariantType();
}

}

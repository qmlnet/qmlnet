#include <QtNetCoreQml/qml/NetVariant.h>
#include <QDateTime>
#include <QDebug>

struct NetInstanceQmlContainer
{
    QSharedPointer<NetInstance> netInstance;
};

Q_DECLARE_METATYPE(NetInstanceQmlContainer)

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
        if(strcmp(variant.typeName(), "NetInstanceQmlContainer") == 0)
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
    variant.setValue(NetInstanceQmlContainer{ netInstance });
}

QSharedPointer<NetInstance> NetVariant::getNetInstance()
{
    return variant.value<NetInstanceQmlContainer>().netInstance;
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
    if(variant.canConvert<NetInstanceQmlContainer>())
    {
        variant.value<NetInstanceQmlContainer>().netInstance.clear();
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

void net_variant_setNetInstance(NetVariantContainer* container, NetInstanceContainer* instanceContainer) {
    if(instanceContainer == NULL) {
        container->variant->setNetInstance(NULL);
    } else {
        container->variant->setNetInstance(instanceContainer->instance);
    }
}

NetInstanceContainer* net_variant_getNetInstance(NetVariantContainer* container) {
    QSharedPointer<NetInstance> instance = container->variant->getNetInstance();
    if(instance == NULL) {
        return NULL;
    }
    NetInstanceContainer* result = new NetInstanceContainer();
    result->instance = instance;
    return result;
}

void net_variant_setBool(NetVariantContainer* container, bool value) {
    container->variant->setBool(value);
}

bool net_variant_getBool(NetVariantContainer* container) {
    return container->variant->getBool();
}

void net_variant_setChar(NetVariantContainer* container, ushort value) {
    container->variant->setChar(value);
}

ushort net_variant_getChar(NetVariantContainer* container) {
    return (ushort)container->variant->getChar().unicode();
}

void net_variant_setInt(NetVariantContainer* container, int value) {
    container->variant->setInt(value);
}

int net_variant_getInt(NetVariantContainer* container) {
    return container->variant->getInt();
}

void net_variant_setUInt(NetVariantContainer* container, unsigned int value) {
    container->variant->setUInt(value);
}

unsigned int net_variant_getUInt(NetVariantContainer* container) {
    return container->variant->getUInt();
}

void net_variant_setDouble(NetVariantContainer* container, double value) {
    container->variant->setDouble(value);
}

double net_variant_getDouble(NetVariantContainer* container) {
    return container->variant->getDouble();
}

NetVariantTypeEnum net_variant_getVariantType(NetVariantContainer* container) {
    return container->variant->getVariantType();
}

}

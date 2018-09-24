#ifndef NETVARIANT_H
#define NETVARIANT_H

#include <QmlNet/types/NetReference.h>
#include <QmlNet/qml/NetJsValue.h>
#include <QVariant>
#include <QDateTime>
#include <QJSValue>

class NetVariant
{
public:
    NetVariant();
    ~NetVariant();
    NetVariantTypeEnum getVariantType();
    void setNetReference(QSharedPointer<NetReference> netReference);
    QSharedPointer<NetReference> getNetReference();
    void setBool(bool value);
    bool getBool();
    void setChar(QChar value);
    QChar getChar();
    void setInt(int value);
    int getInt();
    void setUInt(unsigned int value);
    unsigned int getUInt();
    void setDouble(double value);
    double getDouble();
    void setString(QString* value);
    QString getString();
    void setDateTime(QDateTime& value);
    QDateTime getDateTime();
    void setJsValue(QSharedPointer<NetJSValue> jsValue);
    QSharedPointer<NetJSValue> getJsValue();
    void clear();
    static QSharedPointer<NetVariant> fromQJSValue(const QJSValue& qJsValue);
    QJSValue toQJSValue(QJSEngine* jsEngine);
    static void fromQVariant(const QVariant* variant, QSharedPointer<NetVariant> destination);
    static QSharedPointer<NetVariant> fromQVariant(const QVariant* variant);
    QVariant toQVariant();
    QString getDisplayValue();
private:
    void clearNetReference();
    QVariant variant;
};

struct NetVariantContainer {
    QSharedPointer<NetVariant> variant;
};

#endif // NETVARIANT_H

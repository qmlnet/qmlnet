#ifndef NETVARIANT_H
#define NETVARIANT_H

#include <QmlNet.h>
#include <QSharedPointer>
#include <QVariant>
#include <QDateTime>
#include <QJSValue>

class NetJSValue;
class NetQObject;
class NetReference;
class NetVariantList;

class NetVariant
{
public:
    NetVariant();
    ~NetVariant();
    NetVariantTypeEnum getVariantType() const;
    void setNetReference(QSharedPointer<NetReference> netReference);
    QSharedPointer<NetReference> getNetReference() const;
    void setBool(bool value);
    bool getBool() const;
    void setChar(QChar value);
    QChar getChar() const;
    void setInt(qint32 value);
    qint32 getInt() const;
    void setUInt(quint32 value);
    quint32 getUInt() const;
    void setLong(qint64 value);
    qint64 getLong() const;
    void setULong(quint64 value);
    quint64 getULong() const;
    void setFloat(float value);
    float getFloat() const;
    void setDouble(double value);
    double getDouble() const;
    void setString(const QString* value);
    void setString(const QString& value);
    QString getString() const;
    void setDateTime(const QDateTime& value);
    QDateTime getDateTime() const;
    void setJsValue(QSharedPointer<NetJSValue> jsValue);
    QSharedPointer<NetJSValue> getJsValue() const;
    void setQObject(QSharedPointer<NetQObject> netQObject);
    QSharedPointer<NetQObject> getQObject() const;
    void setNetVariantList(QSharedPointer<NetVariantList> netVariantList);
    QSharedPointer<NetVariantList> getNetVariantList() const;
    void clear();
    static QSharedPointer<NetVariant> fromQJSValue(const QJSValue& qJsValue);
    QJSValue toQJSValue() const;
    static void fromQVariant(const QVariant* variant, const QSharedPointer<NetVariant>& destination);
    static QSharedPointer<NetVariant> fromQVariant(const QVariant* variant);
    QVariant toQVariant() const;
    QString getDisplayValue() const;
private:
    void clearNetReference();

    template<typename T>
    void setValue(const T& value);
    void setValueVariant(const QVariant& value);

    template<typename T>
    void setValuePtr(const T* value);

    template<typename T>
    T getValue() const;

    QVariant variant;
};

struct NetVariantContainer {
    QSharedPointer<NetVariant> variant;
};

#endif // NETVARIANT_H

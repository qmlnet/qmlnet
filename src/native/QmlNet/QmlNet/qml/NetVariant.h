#ifndef NETVARIANT_H
#define NETVARIANT_H

#include <QmlNet.h>
#include <QSharedPointer>
#include <QVariant>
#include <QDateTime>
#include <QJSValue>
#include <QColor>
#include <QRect>
#include <QVector2D>
#include <QVector3D>
#include <QVector4D>
#include <QQuaternion>
#include <QMatrix4x4>

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
    void setNull();
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
    QSize getSize() const;
    void setSize(const QSize &value);
    QSizeF getSizeF() const;
    void setSizeF(const QSizeF &value);
    QRect getRect() const;
    void setRect(const QRect &value);
    QRectF getRectF() const;
    void setRectF(const QRectF &value);
    QPoint getPoint() const;
    void setPoint(const QPoint &value);
    QPointF getPointF() const;
    void setPointF(const QPointF &value);
    QVector2D getVector2D() const;
    void setVector2D(const QVector2D &value);
    QVector3D getVector3D() const;
    void setVector3D(const QVector3D &value);
    QVector4D getVector4D() const;
    void setVector4D(const QVector4D &value);
    QQuaternion getQuaternion() const;
    void setQuaternion(const QQuaternion &value);
    QMatrix4x4 getMatrix4x4() const;
    void setMatrix4x4(const QMatrix4x4 &value);
    QColor getColor() const;
    void setColor(const QColor& value);
    void setString(const QString* value);
    void setString(const QString& value);
    QString getString() const;
    void setBytes(QByteArray byteArray);
    QByteArray getBytes() const;    
    void setDateTime(const QDateTime& value);
    QDateTime getDateTime() const;
    void setJsValue(QSharedPointer<NetJSValue> jsValue);
    QSharedPointer<NetJSValue> getJsValue() const;
    void setQObject(QSharedPointer<NetQObject> netQObject);
    QSharedPointer<NetQObject> getQObject() const;
    void setNetVariantList(QSharedPointer<NetVariantList> netVariantList);
    QSharedPointer<NetVariantList> getNetVariantList() const;
    void clear();

    QVariantList toQVariantList() const;
    static QSharedPointer<NetVariant> fromQJSValue(const QJSValue& qJsValue);
    QJSValue toQJSValue() const;
    static void fromQVariant(const QVariant* variant, const QSharedPointer<NetVariant>& destination);
    static QSharedPointer<NetVariant> fromQVariant(const QVariant* variant);
    QVariant toQVariant() const;
    void toQVariant(QVariant* variant) const;
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

    QVariant _variant;
};

struct NetVariantContainer {
    QSharedPointer<NetVariant> variant;
};

#endif // NETVARIANT_H

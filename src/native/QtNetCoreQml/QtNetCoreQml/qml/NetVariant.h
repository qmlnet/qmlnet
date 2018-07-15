#ifndef NETVARIANT_H
#define NETVARIANT_H

#include <QtNetCoreQml/types/NetInstance.h>
#include <QVariant>

class NetVariant
{
public:
    NetVariant();
    ~NetVariant();
    NetVariantTypeEnum GetVariantType();
    void SetNetInstance(QSharedPointer<NetInstance> netInstance);
    QSharedPointer<NetInstance> GetNetInstance();
    void SetBool(bool value);
    bool GetBool();
    void SetChar(QChar value);
    QChar GetChar();
    void SetInt(int value);
    int GetInt();
    void SetUInt(unsigned int value);
    unsigned int GetUInt();
    void SetDouble(double value);
    double GetDouble();
    void SetString(QString* value);
    QString GetString();
    void SetDateTime(QDateTime& value);
    QDateTime GetDateTime();
    void Clear();

    QVariant AsQVariant();
private:
    void ClearNetInstance();
    QVariant variant;
};

#endif // NETVARIANT_H

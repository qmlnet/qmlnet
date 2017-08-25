#ifndef NET_VARIANT_H
#define NET_VARIANT_H

#include "net_instance.h"
#include <QVariant>

class NetVariant
{
public:
    NetVariant();
    ~NetVariant();
    NetVariantTypeEnum GetVariantType();
    void SetNetInstance(NetInstance* netInstance);
    NetInstance* GetNetInstance();
    void SetBool(bool value);
    bool GetBool();
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
private:
    void ClearNetInstance();
    QVariant variant;
};

#endif // NET_VARIANT_H

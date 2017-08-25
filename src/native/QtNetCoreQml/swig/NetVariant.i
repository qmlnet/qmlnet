%{
#include "net_variant.h"
%}

class NetVariant
{
public:
    NetVariant();
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
}; 
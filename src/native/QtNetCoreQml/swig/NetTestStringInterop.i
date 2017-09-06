%{
#include "net_test_string_interop.h"
%}

class NetTestStringInterop
{
public:
    NetTestStringInterop();
    void SetStringValue(QString value);
    QString GetStringValue();
    void SetStringReference(QString& value);
    QString& GetStringReference();
    void SetStringPointer(QString* value);
    QString* GetStringPointer();
    void PrintString();
private:
    QString _value;
};
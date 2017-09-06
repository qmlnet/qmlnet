#ifndef NET_TEST_STRING_INTEROP_H
#define NET_TEST_STRING_INTEROP_H

#include <QString>
#include <QDebug>

class NetTestStringInterop
{
public:
    NetTestStringInterop();
    void SetStringValue(QString value) {
        _value = value;
    }
    QString GetStringValue() {
        return _value;
    }
    void SetStringReference(QString& value) {
        _value = value;
    }
    QString& GetStringReference() {
        return _value;
    }
    void SetStringPointer(QString* value) {
        if(value) {
            _value = *value;
        } else {
            _value.clear();
        }
    }
    QString* GetStringPointer() {
        return &_value;
    }

    void PrintString() {
        qDebug() << _value;
    }

private:
    QString _value;
};

#endif // NET_TEST_STRING_INTEROP_H

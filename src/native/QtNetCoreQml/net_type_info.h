#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <string>
#include <QDebug>

class NetMethodInfo
{
public:
    NetMethodInfo();
    ~NetMethodInfo(){
        qDebug() << "dstr";
    }
private:
};

#endif // NET_TYPE_INFO_H

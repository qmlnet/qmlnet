#ifndef NET_QML_REGISTER_TYPE_H
#define NET_QML_REGISTER_TYPE_H

#include <string>

//#include <QObject>
//
//#include "private/qobject_p.h"

//class NetInstanceObject : public QObject
//{
//    Q_OBJECT
//public:
//    NetInstanceObject(QObject *parent);
//    virtual ~NetInstanceObject();
//};

//template <int N>
//class NetInstanceObjectType : public NetInstanceObject
//{
//public:

//    NetInstanceObjectType()
//        : NetInstanceObject(0) {}

//    static void init(std::string &netType)
//    {
//        static_cast<QMetaObject &>(staticMetaObject) = *metaObjectFor(netType);
//    };

//    static QMetaObject staticMetaObject;
//};

int registerNetType(std::string netType, std::string uri, int versionMajor, int versionMinor, std::string qmlName);

#endif // NET_QML_REGISTER_TYPE_H

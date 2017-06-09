#ifndef NET_QML_VALUE_TYPE_H
#define NET_QML_VALUE_TYPE_H

#include "qtnetcoreqml_global.h"
#include "qobject.h"

template <int N>
class GoValueType : public GoValue
{
public:

    GoValueType()
        : GoValue(NULL, typeInfo, 0) {};

    static void init(GoTypeInfo *info, GoTypeSpec_ *spec)
    {
        typeInfo = info;
        typeSpec = spec;
        static_cast<QMetaObject &>(staticMetaObject) = *metaObjectFor(typeInfo);
    };

    static GoTypeSpec_ *typeSpec;
    static GoTypeInfo *typeInfo;
    static QMetaObject staticMetaObject;
};

#endif // NET_QML_VALUE_TYPE_H

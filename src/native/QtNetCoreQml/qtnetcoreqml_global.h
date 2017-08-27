#ifndef QTNETCOREQML_GLOBAL_H
#define QTNETCOREQML_GLOBAL_H

#include <QtCore/qglobal.h>

enum NetVariantTypeEnum {
    NetVariantTypeEnum_Invalid,
    NetVariantTypeEnum_Bool,
    NetVariantTypeEnum_Char,
    NetVariantTypeEnum_Int,
    NetVariantTypeEnum_UInt,
    NetVariantTypeEnum_Double,
    NetVariantTypeEnum_String,
    NetVariantTypeEnum_DateTime,
    NetVariantTypeEnum_Object
};

#define NetGCHandle void

#endif // QTNETCOREQML_GLOBAL_H

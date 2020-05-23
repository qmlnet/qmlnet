#ifndef QMLNET_GLOBAL_H
#define QMLNET_GLOBAL_H

#include <QtCore/qglobal.h>

#define NetGCHandle void

enum NetVariantTypeEnum {
    NetVariantTypeEnum_Invalid = 0,
    NetVariantTypeEnum_Null,
    NetVariantTypeEnum_Bool,
    NetVariantTypeEnum_Char,
    NetVariantTypeEnum_Int,
    NetVariantTypeEnum_UInt,
    NetVariantTypeEnum_Long,
    NetVariantTypeEnum_ULong,
    NetVariantTypeEnum_Float,
    NetVariantTypeEnum_Double,
    NetVariantTypeEnum_String,
    NetVariantTypeEnum_DateTime,
    NetVariantTypeEnum_Object,
    NetVariantTypeEnum_JSValue,
    NetVariantTypeEnum_QObject,
    NetVariantTypeEnum_NetVariantList,
    NetVariantTypeEnum_ByteArray
};


#endif // QMLNET_GLOBAL_H

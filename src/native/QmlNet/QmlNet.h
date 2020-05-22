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
    NetVariantTypeEnum_ByteArray,
    NetVariantTypeEnum_Size,
    NetVariantTypeEnum_SizeF,
    NetVariantTypeEnum_Rect,
    NetVariantTypeEnum_RectF,
    NetVariantTypeEnum_Point,
    NetVariantTypeEnum_PointF,
    NetVariantTypeEnum_Color,
    NetVariantTypeEnum_Vector2D,
    NetVariantTypeEnum_Vector3D,
    NetVariantTypeEnum_Vector4D,
    NetVariantTypeEnum_Quaternion,
    NetVariantTypeEnum_Matrix4x4,
};


#endif // QMLNET_GLOBAL_H

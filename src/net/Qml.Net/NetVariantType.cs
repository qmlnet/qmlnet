namespace Qml.Net
{
    public enum NetVariantType
    {
        Invalid = 0,
        Null,
        Bool,
        Char,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        String,
        DateTime,
        Object,
        JsValue,
        QObject,
        NetVariantList,
        ByteArray,
        Size,
        SizeF,
        Rect,
        RectF,
        Point,
        PointF,
        Color,
#if NETSTANDARD2_1
        Vector2D,
        Vector3D,
        Vector4D,
        Quaternion,
        Matrix4x4,
#endif
    }
}
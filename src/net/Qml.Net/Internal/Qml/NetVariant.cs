using System;
using System.Collections.Generic;
using System.Drawing;
#if NETSTANDARD2_1
using System.Numerics;
#endif
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.Qml
{
    internal class NetVariant : BaseDisposable
    {
        public NetVariant()
            : this(Interop.NetVariant.Create())
        {
        }

        public NetVariant(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
        }

        public NetVariantType VariantType => Interop.NetVariant.GetVariantType(Handle);

        public void SetNull()
        {
            Interop.NetVariant.SetNull(Handle);
        }
        
        public NetReference Instance
        {
            get
            {
                var result = Interop.NetVariant.GetNetReference(Handle);
                return result == IntPtr.Zero ? null : new NetReference(result);
            }
            set => Interop.NetVariant.SetNetReference(Handle, value?.Handle ?? IntPtr.Zero);
        }

        public bool Bool
        {
            get => Interop.NetVariant.GetBool(Handle) == 1;
            set => Interop.NetVariant.SetBool(Handle, value ? (byte)1 : (byte)0);
        }

        public char Char
        {
            get => (char)Interop.NetVariant.GetChar(Handle);
            set => Interop.NetVariant.SetChar(Handle, value);
        }

        public int Int
        {
            get => Interop.NetVariant.GetInt(Handle);
            set => Interop.NetVariant.SetInt(Handle, value);
        }

        public uint UInt
        {
            get => Interop.NetVariant.GetUInt(Handle);
            set => Interop.NetVariant.SetUInt(Handle, value);
        }

        public long Long
        {
            get => Interop.NetVariant.GetLong(Handle);
            set => Interop.NetVariant.SetLong(Handle, value);
        }

        public ulong ULong
        {
            get => Interop.NetVariant.GetULong(Handle);
            set => Interop.NetVariant.SetULong(Handle, value);
        }

        public float Float
        {
            get => Interop.NetVariant.GetFloat(Handle);
            set => Interop.NetVariant.SetFloat(Handle, value);
        }

        public double Double
        {
            get => Interop.NetVariant.GetDouble(Handle);
            set => Interop.NetVariant.SetDouble(Handle, value);
        }

        public Size Size
        {
            get
            {
                Interop.NetVariant.GetSize(Handle, out var w, out var h);
                return new Size(w, h);
            }
            set => Interop.NetVariant.SetSize(Handle, value.Width, value.Height);
        }

        public SizeF SizeF
        {
            get
            {
                Interop.NetVariant.GetSizeF(Handle, out var w, out var h);
                return new SizeF(w, h);
            }
            set => Interop.NetVariant.SetSizeF(Handle, value.Width, value.Height);
        }

        public Rectangle Rect
        {
            get
            {
                Interop.NetVariant.GetRect(Handle, out var x, out var y, out var w, out var h);
                return new Rectangle(x, y, w, h);
            }
            set => Interop.NetVariant.SetRect(Handle, value.X, value.Y, value.Width, value.Height);
        }

        public RectangleF RectF
        {
            get
            {
                Interop.NetVariant.GetRectF(Handle, out var x, out var y, out var w, out var h);
                return new RectangleF(x, y, w, h);
            }
            set => Interop.NetVariant.SetRectF(Handle, value.X, value.Y, value.Width, value.Height);
        }

        public Point Point
        {
            get
            {
                Interop.NetVariant.GetPoint(Handle, out var x, out var y);
                return new Point(x, y);
            }
            set => Interop.NetVariant.SetPoint(Handle, value.X, value.Y);
        }

        public PointF PointF
        {
            get
            {
                Interop.NetVariant.GetPointF(Handle, out var x, out var y);
                return new PointF(x, y);
            }
            set => Interop.NetVariant.SetPointF(Handle, value.X, value.Y);
        }
        
#if NETSTANDARD2_1
        public Vector2 Vector2D
        {
            get
            {
                Interop.NetVariant.GetVector2D(Handle, out var x, out var y);
                return new Vector2(x, y);
            }
            set => Interop.NetVariant.SetVector2D(Handle, value.X, value.Y);
        }

        public Vector3 Vector3D
        {
            get
            {
                Interop.NetVariant.GetVector3D(Handle, out var x, out var y, out var z);
                return new Vector3(x, y, z);
            }
            set => Interop.NetVariant.SetVector3D(Handle, value.X, value.Y, value.Z);
        }

        public Vector4 Vector4D
        {
            get
            {
                Interop.NetVariant.GetVector4D(Handle, out var x, out var y, out var z, out var w);
                return new Vector4(x, y, z, w);
            }
            set => Interop.NetVariant.SetVector4D(Handle, value.X, value.Y, value.Z, value.W);
        }

        public Quaternion Quaternion
        {
            get
            {
                Interop.NetVariant.GetQuaternion(Handle, out var w, out var x, out var y, out var z);
                return new Quaternion(x, y, z, w);
            }
            set => Interop.NetVariant.SetQuaternion(Handle, value.W, value.X, value.Y, value.Z);
        }

        public Matrix4x4 Matrix4x4
        {
            get
            {
                Interop.NetVariant.GetMatrix4x4(Handle, out var m11, out var m12, out var m13, out var m14, out var m21, out var m22, out var m23, out var m24, out var m31, out var m32, out var m33, out var m34, out var m41, out var m42, out var m43, out var m44);
                return new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
            }
            set => Interop.NetVariant.SetMatrix4x4(Handle, value.M11, value.M12, value.M13, value.M14, value.M21, value.M22, value.M23, value.M24, value.M31, value.M32, value.M33, value.M34, value.M41, value.M42, value.M43, value.M44);
        }
#endif

        public System.Drawing.Color Color
        {
            get
            {
                var colorContainer = new ColorContainer();
                Interop.NetVariant.GetColor(Handle, ref colorContainer);
                if (colorContainer.IsNull == 1)
                {
                    return default;
                }
                return System.Drawing.Color.FromArgb(colorContainer.Alpha, colorContainer.Red, colorContainer.Green, colorContainer.Blue);
            }

            set
            {
                var colorContainer = new ColorContainer();
                if (value.IsEmpty)
                {
                    colorContainer.IsNull = 1;
                }
                else
                {
                    colorContainer.IsNull = 0;
                    colorContainer.Red = value.R;
                    colorContainer.Green = value.G;
                    colorContainer.Blue = value.B;
                    colorContainer.Alpha = value.A;
                }
                Interop.NetVariant.SetColor(Handle, ref colorContainer);
            }
        }

        public string String
        {
            get => Utilities.ContainerToString(Interop.NetVariant.GetString(Handle));
            set => Interop.NetVariant.SetString(Handle, value);
        }

        public byte[] ByteArray
        {
            get 
            {
                IntPtr arrayHandle = Interop.NetVariant.GetBytes(Handle, out var count);
                if (arrayHandle == IntPtr.Zero)
                {
                    return null;
                }
                byte[] managedArray = new byte[count];
                Marshal.Copy(arrayHandle, managedArray, 0, count);
                return managedArray;
            }
            
            set
            {
                if (value == null)
                {
                    Interop.NetVariant.SetBytes(Handle, IntPtr.Zero, 0);
                }
                else
                {
                    GCHandle pinnedArray = GCHandle.Alloc(value, GCHandleType.Pinned);
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                    Interop.NetVariant.SetBytes(Handle, pointer, value.Length);
                }
            }
        }
        
        public DateTimeOffset? DateTime
        {
            get
            {
                var dateTime = new DateTimeContainer();
                Interop.NetVariant.GetDateTime(Handle, ref dateTime);
                if (dateTime.IsNull == 1)
                    return null;
                return new DateTimeOffset(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day,
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second,
                    dateTime.Msec,
                    TimeSpan.FromSeconds(dateTime.OffsetSeconds));
            }

            set
            {
                var dateTime = new DateTimeContainer();
                if (value == null)
                {
                    dateTime.IsNull = 1;
                    Interop.NetVariant.SetDateTime(Handle, ref dateTime);
                }
                else
                {
                    dateTime.IsNull = 0;
                    dateTime.Year = value.Value.Year;
                    dateTime.Month = value.Value.Month;
                    dateTime.Day = value.Value.Day;
                    dateTime.Hour = value.Value.Hour;
                    dateTime.Minute = value.Value.Minute;
                    dateTime.Second = value.Value.Second;
                    dateTime.Msec = value.Value.Millisecond;
                    dateTime.OffsetSeconds = (int)value.Value.Offset.TotalSeconds;
                    Interop.NetVariant.SetDateTime(Handle, ref dateTime);
                }
            }
        }

        public NetJsValue JsValue
        {
            get
            {
                var result = Interop.NetVariant.GetJsValue(Handle);
                return result == IntPtr.Zero ? null : new NetJsValue(result);
            }
            set => Interop.NetVariant.SetJsValue(Handle, value?.Handle ?? IntPtr.Zero);
        }

        public NetQObject QObject
        {
            get
            {
                var result = Interop.NetVariant.GetQObject(Handle);
                return result == IntPtr.Zero ? null : new NetQObject(result);
            }
            set => Interop.NetVariant.SetQObject(Handle, value?.Handle ?? IntPtr.Zero);
        }

        public NetVariantList NetVariantList
        {
            get
            {
                var result = Interop.NetVariant.GetNetVariantList(Handle);
                if (result == IntPtr.Zero)
                {
                    return null;
                }
                return new NetVariantList(result);
            }
            set => Interop.NetVariant.SetNetVariantList(Handle, value?.Handle ?? IntPtr.Zero);
        }

        public void Clear()
        {
            Interop.NetVariant.Clear(Handle);
        }

        public object AsObject()
        {
            switch (VariantType)
            {
                case NetVariantType.Invalid:
                    return null;
                case NetVariantType.Bool:
                    return Bool;
                case NetVariantType.Char:
                    return Char;
                case NetVariantType.Int:
                    return Int;
                case NetVariantType.UInt:
                    return UInt;
                case NetVariantType.Long:
                    return Long;
                case NetVariantType.ULong:
                    return ULong;
                case NetVariantType.Float:
                    return Float;
                case NetVariantType.Double:
                    return Double;
                case NetVariantType.Size:
                    return Size;
                case NetVariantType.SizeF:
                    return SizeF;
                case NetVariantType.Rect:
                    return Rect;
                case NetVariantType.RectF:
                    return RectF;
                case NetVariantType.Point:
                    return Point;
                case NetVariantType.PointF:
                    return PointF;
#if NETSTANDARD2_1
                case NetVariantType.Vector2D:
                    return Vector2D;
                case NetVariantType.Vector3D:
                    return Vector3D;
                case NetVariantType.Vector4D:
                    return Vector4D;
                case NetVariantType.Quaternion:
                    return Quaternion;
                case NetVariantType.Matrix4x4:
                    return Matrix4x4;
#endif
                case NetVariantType.Color:
                    return Color;
                case NetVariantType.String:
                    return String;
                case NetVariantType.DateTime:
                    return DateTime;
                case NetVariantType.ByteArray:
                    return ByteArray;                    
                case NetVariantType.Object:
                    using (var instance = Instance)
                    {
                        return instance.Instance;
                    }

                case NetVariantType.JsValue:
                    return JsValue.AsDynamic();
                case NetVariantType.QObject:
                    return QObject.AsDynamic();
                case NetVariantType.NetVariantList:
                    using (var netVariantList = NetVariantList)
                    {
                        if (netVariantList == null)
                        {
                            return null;
                        }

                        var result = new List<object>();
                        var count = netVariantList.Count;
                        for (var x = 0; x < count; x++)
                        {
                            using (var value = netVariantList.Get(x))
                            {
                                result.Add(value.AsObject());
                            }
                        }

                        return result;
                    }
                default:
                    throw new NotImplementedException($"unhandled type {VariantType}");
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }

        public static NetVariant From<T>(T value)
        {
            var variant = new NetVariant();
            if (value != null)
            {
                Helpers.Pack(value, variant, typeof(T));
            }
            return variant;
        }

        public static NetVariant From(object value)
        {
            var variant = new NetVariant();
            if (value != null)
            {
                Helpers.Pack(value, variant, value.GetType());
            }
            return variant;
        }
    }

    internal class NetVariantInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_create")]
        public CreateDel Create { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr CreateDel();

        [NativeSymbol(Entrypoint = "net_variant_destroy")]
        public DestroyDel Destroy { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DestroyDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_getVariantType")]
        public GetVariantTypeDel GetVariantType { get; set; }

        [NativeSymbol(Entrypoint = "net_variant_setNull")]
        public SetNullDel SetNull { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetNullDel(IntPtr variant);
        
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate NetVariantType GetVariantTypeDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setNetReference")]
        public SetNetReferenceDel SetNetReference { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetNetReferenceDel(IntPtr variant, IntPtr instance);

        [NativeSymbol(Entrypoint = "net_variant_getNetReference")]
        public GetNetReferenceDel GetNetReference { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetNetReferenceDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setBool")]
        public SetBoolDel SetBool { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBoolDel(IntPtr variant, byte value);

        [NativeSymbol(Entrypoint = "net_variant_getBool")]
        public GetBoolDel GetBool { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte GetBoolDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setChar")]
        public SetCharDel SetChar { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetCharDel(IntPtr variant, ushort value);

        [NativeSymbol(Entrypoint = "net_variant_getChar")]
        public GetCharDel GetChar { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ushort GetCharDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setInt")]
        public SetIntDel SetInt { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetIntDel(IntPtr variant, int value);

        [NativeSymbol(Entrypoint = "net_variant_getInt")]
        public GetIntDel GetInt { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetIntDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setUInt")]
        public SetUIntDel SetUInt { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetUIntDel(IntPtr variant, uint value);

        [NativeSymbol(Entrypoint = "net_variant_getUInt")]
        public GetUIntDel GetUInt { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint GetUIntDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setLong")]
        public SetLongDel SetLong { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetLongDel(IntPtr variant, long value);

        [NativeSymbol(Entrypoint = "net_variant_getLong")]
        public GetLongDel GetLong { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long GetLongDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setULong")]
        public SetULongDel SetULong { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetULongDel(IntPtr variant, ulong value);

        [NativeSymbol(Entrypoint = "net_variant_getULong")]
        public GetULongDel GetULong { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ulong GetULongDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setFloat")]
        public SetFloatDel SetFloat { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFloatDel(IntPtr variant, float value);

        [NativeSymbol(Entrypoint = "net_variant_getFloat")]
        public GetFloatDel GetFloat { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate float GetFloatDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setDouble")]
        public SetDoubleDel SetDouble { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetDoubleDel(IntPtr variant, double value);

        [NativeSymbol(Entrypoint = "net_variant_getDouble")]
        public GetDoubleDel GetDouble { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double GetDoubleDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setSize")]
        public SetSizeDel SetSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetSizeDel(IntPtr variant, int w, int h);

        [NativeSymbol(Entrypoint = "net_variant_getSize")]
        public GetSizeDel GetSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetSizeDel(IntPtr variant, out int w, out int h);

        [NativeSymbol(Entrypoint = "net_variant_setSizeF")]
        public SetSizeFDel SetSizeF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetSizeFDel(IntPtr variant, float w, float h);

        [NativeSymbol(Entrypoint = "net_variant_getSizeF")]
        public GetSizeFDel GetSizeF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetSizeFDel(IntPtr variant, out float w, out float h);

        [NativeSymbol(Entrypoint = "net_variant_setRect")]
        public SetRectDel SetRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetRectDel(IntPtr variant, int x, int y, int w, int h);

        [NativeSymbol(Entrypoint = "net_variant_getRect")]
        public GetRectDel GetRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetRectDel(IntPtr variant, out int x, out int y, out int w, out int h);

        [NativeSymbol(Entrypoint = "net_variant_setRectF")]
        public SetRectFDel SetRectF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetRectFDel(IntPtr variant, float x, float y, float w, float h);

        [NativeSymbol(Entrypoint = "net_variant_getRectF")]
        public GetRectFDel GetRectF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetRectFDel(IntPtr variant, out float x, out float y, out float w, out float h);

        [NativeSymbol(Entrypoint = "net_variant_setPoint")]
        public SetPointDel SetPoint { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPointDel(IntPtr variant, int x, int y);

        [NativeSymbol(Entrypoint = "net_variant_getPoint")]
        public GetPointDel GetPoint { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetPointDel(IntPtr variant, out int x, out int y);

        [NativeSymbol(Entrypoint = "net_variant_setPointF")]
        public SetPointFDel SetPointF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPointFDel(IntPtr variant, float x, float y);

        [NativeSymbol(Entrypoint = "net_variant_getPointF")]
        public GetPointFDel GetPointF { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetPointFDel(IntPtr variant, out float x, out float y);

        [NativeSymbol(Entrypoint = "net_variant_setVector2D")]
        public SetVector2DDel SetVector2D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetVector2DDel(IntPtr variant, float x, float y);

        [NativeSymbol(Entrypoint = "net_variant_getVector2D")]
        public GetVector2DDel GetVector2D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetVector2DDel(IntPtr variant, out float x, out float y);

        [NativeSymbol(Entrypoint = "net_variant_setVector3D")]
        public SetVector3DDel SetVector3D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetVector3DDel(IntPtr variant, float x, float y, float z);

        [NativeSymbol(Entrypoint = "net_variant_getVector3D")]
        public GetVector3DDel GetVector3D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetVector3DDel(IntPtr variant, out float x, out float y, out float z);

        [NativeSymbol(Entrypoint = "net_variant_setVector4D")]
        public SetVector4DDel SetVector4D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetVector4DDel(IntPtr variant, float x, float y, float z, float w);

        [NativeSymbol(Entrypoint = "net_variant_getVector4D")]
        public GetVector4DDel GetVector4D { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetVector4DDel(IntPtr variant, out float x, out float y, out float z, out float w);

        [NativeSymbol(Entrypoint = "net_variant_setQuaternion")]
        public SetQuaternionDel SetQuaternion { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetQuaternionDel(IntPtr variant, float w, float x, float y, float z);

        [NativeSymbol(Entrypoint = "net_variant_getQuaternion")]
        public GetQuaternionDel GetQuaternion { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetQuaternionDel(IntPtr variant, out float w, out float x, out float y, out float z);

        [NativeSymbol(Entrypoint = "net_variant_setMatrix4x4")]
        public SetMatrix4x4Del SetMatrix4x4 { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetMatrix4x4Del(IntPtr variant, float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44);

        [NativeSymbol(Entrypoint = "net_variant_getMatrix4x4")]
        public GetMatrix4x4Del GetMatrix4x4 { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetMatrix4x4Del(IntPtr variant, out float m11, out float m12, out float m13, out float m14, out float m21, out float m22, out float m23, out float m24, out float m31, out float m32, out float m33, out float m34, out float m41, out float m42, out float m43, out float m44);

        [NativeSymbol(Entrypoint = "net_variant_setColor")]
        public SetColorDel SetColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetColorDel(IntPtr variant, ref ColorContainer color);

        [NativeSymbol(Entrypoint = "net_variant_getColor")]
        public GetColorDel GetColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetColorDel(IntPtr variant, ref ColorContainer color);

        [NativeSymbol(Entrypoint = "net_variant_setString")]
        public SetStringDel SetString { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetStringDel(IntPtr variant, [MarshalAs(UnmanagedType.LPWStr)]string value);

        [NativeSymbol(Entrypoint = "net_variant_getString")]
        public GetStringDel GetString { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetStringDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setBytes")]
        public SetBytesDel SetBytes { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBytesDel(IntPtr variant, IntPtr array, int count);

        [NativeSymbol(Entrypoint = "net_variant_getBytes")]
        public GetBytesDel GetBytes { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetBytesDel(IntPtr variant, out int count);

        [NativeSymbol(Entrypoint = "net_variant_setDateTime")]
        public SetDateTimeDel SetDateTime { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetDateTimeDel(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_getDateTime")]
        public GetDateTimeDel GetDateTime { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetDateTimeDel(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_setJsValue")]
        public SetJsValueDel SetJsValue { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetJsValueDel(IntPtr variant, IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_variant_getJsValue")]
        public GetJsValueDel GetJsValue { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetJsValueDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setQObject")]
        public SetQObjectDel SetQObject { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetQObjectDel(IntPtr variant, IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_variant_getQObject")]
        public GetQObjectDel GetQObject { get; set; }
        
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetQObjectDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setNetVariantList")]
        public SetNetVariantListDel SetNetVariantList { get; set; }
        
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetNetVariantListDel(IntPtr variant, IntPtr variantList);
        
        [NativeSymbol(Entrypoint = "net_variant_getNetVariantList")]
        public GetNetVariantListDel GetNetVariantList { get; set; }
        
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetNetVariantListDel(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_clear")]
        public ClearDel Clear { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ClearDel(IntPtr variant);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DateTimeContainer
    {
        public byte IsNull;
        public int Month;
        public int Day;
        public int Year;
        public int Hour;
        public int Minute;
        public int Second;
        public int Msec;
        public int OffsetSeconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorContainer
    {
        public byte IsNull;
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;
    }
}
using System;
using System.Collections.Generic;
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

        public string String
        {
            get => Utilities.ContainerToString(Interop.NetVariant.GetString(Handle));
            set => Interop.NetVariant.SetString(Handle, value);
        }

        public byte[] ByteArray
        {
            get {
                int count;
                IntPtr arrayHandle = Interop.NetVariant.GetBytes(Handle, out count);
                byte[] managedArray = new byte[count];
                Marshal.Copy(arrayHandle, managedArray, 0, count);
                return managedArray;
            } 
            set {
                GCHandle pinnedArray = GCHandle.Alloc(value, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                Interop.NetVariant.SetBytes(Handle, pointer, value.Length);
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
}
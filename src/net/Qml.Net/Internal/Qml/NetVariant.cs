using System;
using System.Runtime.InteropServices;
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
            get => Interop.NetVariant.GetBool(Handle);
            set => Interop.NetVariant.SetBool(Handle, value);
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

        public DateTimeOffset? DateTime
        {
            get
            {
                var dateTime = new DateTimeContainer();
                Interop.NetVariant.GetDateTime(Handle, ref dateTime);
                if (dateTime.IsNull)
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
                    dateTime.IsNull = true;
                    Interop.NetVariant.SetDateTime(Handle, ref dateTime);
                }
                else
                {
                    dateTime.IsNull = false;
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

        public void Clear()
        {
            Interop.NetVariant.Clear(Handle);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }
    }

    internal class NetVariantInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel();

        [NativeSymbol(Entrypoint = "net_variant_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_getVariantType")]
        public GetVariantTypeDel GetVariantType { get; set; }

        public delegate NetVariantType GetVariantTypeDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setNetReference")]
        public SetNetReferenceDel SetNetReference { get; set; }

        public delegate void SetNetReferenceDel(IntPtr variant, IntPtr instance);

        [NativeSymbol(Entrypoint = "net_variant_getNetReference")]
        public GetNetReferenceDel GetNetReference { get; set; }

        public delegate IntPtr GetNetReferenceDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setBool")]
        public SetBoolDel SetBool { get; set; }

        public delegate void SetBoolDel(IntPtr variant, bool value);

        [NativeSymbol(Entrypoint = "net_variant_getBool")]
        public GetBoolDel GetBool { get; set; }

        public delegate bool GetBoolDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setChar")]
        public SetCharDel SetChar { get; set; }

        public delegate void SetCharDel(IntPtr variant, ushort value);

        [NativeSymbol(Entrypoint = "net_variant_getChar")]
        public GetCharDel GetChar { get; set; }

        public delegate ushort GetCharDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setInt")]
        public SetIntDel SetInt { get; set; }

        public delegate void SetIntDel(IntPtr variant, int value);

        [NativeSymbol(Entrypoint = "net_variant_getInt")]
        public GetIntDel GetInt { get; set; }

        public delegate int GetIntDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setUInt")]
        public SetUIntDel SetUInt { get; set; }

        public delegate void SetUIntDel(IntPtr variant, uint value);

        [NativeSymbol(Entrypoint = "net_variant_getUInt")]
        public GetUIntDel GetUInt { get; set; }

        public delegate uint GetUIntDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setLong")]
        public SetLongDel SetLong { get; set; }

        public delegate void SetLongDel(IntPtr variant, long value);

        [NativeSymbol(Entrypoint = "net_variant_getLong")]
        public GetLongDel GetLong { get; set; }

        public delegate long GetLongDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setULong")]
        public SetULongDel SetULong { get; set; }

        public delegate void SetULongDel(IntPtr variant, ulong value);

        [NativeSymbol(Entrypoint = "net_variant_getULong")]
        public GetULongDel GetULong { get; set; }

        public delegate ulong GetULongDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setFloat")]
        public SetFloatDel SetFloat { get; set; }

        public delegate void SetFloatDel(IntPtr variant, float value);

        [NativeSymbol(Entrypoint = "net_variant_getFloat")]
        public GetFloatDel GetFloat { get; set; }

        public delegate float GetFloatDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setDouble")]
        public SetDoubleDel SetDouble { get; set; }

        public delegate void SetDoubleDel(IntPtr variant, double value);

        [NativeSymbol(Entrypoint = "net_variant_getDouble")]
        public GetDoubleDel GetDouble { get; set; }

        public delegate double GetDoubleDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setString")]
        public SetStringDel SetString { get; set; }

        public delegate void SetStringDel(IntPtr variant, [MarshalAs(UnmanagedType.LPWStr)]string value);

        [NativeSymbol(Entrypoint = "net_variant_getString")]
        public GetStringDel GetString { get; set; }

        public delegate IntPtr GetStringDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setDateTime")]
        public SetDateTimeDel SetDateTime { get; set; }

        public delegate void SetDateTimeDel(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_getDateTime")]
        public GetDateTimeDel GetDateTime { get; set; }

        public delegate void GetDateTimeDel(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_setJsValue")]
        public SetJsValueDel SetJsValue { get; set; }

        public delegate void SetJsValueDel(IntPtr variant, IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_variant_getJsValue")]
        public GetJsValueDel GetJsValue { get; set; }

        public delegate IntPtr GetJsValueDel(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_clear")]
        public ClearDel Clear { get; set; }

        public delegate void ClearDel(IntPtr variant);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DateTimeContainer
    {
        public bool IsNull;
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
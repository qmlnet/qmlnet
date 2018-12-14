using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
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
                return new DateTimeOffset(dateTime.Year,
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
    
    internal interface INetVariantInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "net_variant_destroy")]
        void Destroy(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_getVariantType")]
        NetVariantType GetVariantType(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setNetReference")]
        void SetNetReference(IntPtr variant, IntPtr instance);
        [NativeSymbol(Entrypoint = "net_variant_getNetReference")]
        IntPtr GetNetReference(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setBool")]
        void SetBool(IntPtr variant, bool value);
        [NativeSymbol(Entrypoint = "net_variant_getBool")]
        bool GetBool(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setChar")]
        void SetChar(IntPtr variant, ushort value);
        [NativeSymbol(Entrypoint = "net_variant_getChar")]
        ushort GetChar(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setInt")]
        void SetInt(IntPtr variant, int value);
        [NativeSymbol(Entrypoint = "net_variant_getInt")]
        int GetInt(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setUInt")]
        void SetUInt(IntPtr variant, uint value);
        [NativeSymbol(Entrypoint = "net_variant_getUInt")]
        uint GetUInt(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setLong")]
        void SetLong(IntPtr variant, long value);
        [NativeSymbol(Entrypoint = "net_variant_getLong")]
        long GetLong(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setULong")]
        void SetULong(IntPtr variant, ulong value);
        [NativeSymbol(Entrypoint = "net_variant_getULong")]
        ulong GetULong(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setFloat")]
        void SetFloat(IntPtr variant, float value);
        [NativeSymbol(Entrypoint = "net_variant_getFloat")]
        float GetFloat(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setDouble")]
        void SetDouble(IntPtr variant, double value);
        [NativeSymbol(Entrypoint = "net_variant_getDouble")]
        double GetDouble(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setString")]
        void SetString(IntPtr variant, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string value);
        [NativeSymbol(Entrypoint = "net_variant_getString")]
        IntPtr GetString(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setDateTime")]
        void SetDateTime(IntPtr variant, ref DateTimeContainer dateTime);
        [NativeSymbol(Entrypoint = "net_variant_getDateTime")]
        void GetDateTime(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_setJsValue")]
        void SetJsValue(IntPtr variant, IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_variant_getJsValue")]
        IntPtr GetJsValue(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_clear")]
        void Clear(IntPtr variant);
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
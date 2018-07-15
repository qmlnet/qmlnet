using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;
using Qt.NetCore.Types;

namespace Qt.NetCore.Qml
{
    public class NetVariant : BaseDisposable
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

        public NetInstance Instance
        {
            get
            {
                var result = Interop.NetVariant.GetNetInstance(Handle);
                return result == IntPtr.Zero ? null : new NetInstance(result);
            }
            set => Interop.NetVariant.SetNetInstance(Handle, value?.Handle ?? IntPtr.Zero);
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
        
        public double Double
        {
            get => Interop.NetVariant.GetDouble(Handle);
            set => Interop.NetVariant.SetDouble(Handle, value);
        }
        
        public string String 
        {
            get => Interop.NetVariant.GetString(Handle);
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
                    dateTime.Minute,
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

        public void Clear()
        {
            Interop.NetVariant.Clear(Handle);
        }
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }
    }
    
    public interface INetVariantInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "net_variant_destroy")]
        void Destroy(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_getVariantType")]
        NetVariantType GetVariantType(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setNetInstance")]
        void SetNetInstance(IntPtr variant, IntPtr instance);
        [NativeSymbol(Entrypoint = "net_variant_getNetInstance")]
        IntPtr GetNetInstance(IntPtr variant);

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
        
        [NativeSymbol(Entrypoint = "net_variant_setDouble")]
        void SetDouble(IntPtr variant, double value);
        [NativeSymbol(Entrypoint = "net_variant_getDouble")]
        double GetDouble(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setString")]
        void SetString(IntPtr variant, [MarshalAs(UnmanagedType.LPWStr)]string value);
        [NativeSymbol(Entrypoint = "net_variant_getString")]
        [return:MarshalAs(UnmanagedType.LPWStr)]string GetString(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_setDateTime")]
        void SetDateTime(IntPtr variant, ref DateTimeContainer dateTime);
        [NativeSymbol(Entrypoint = "net_variant_getDateTime")]
        void GetDateTime(IntPtr variant, ref DateTimeContainer dateTime);

        [NativeSymbol(Entrypoint = "net_variant_clear")]
        void Clear(IntPtr variant);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DateTimeContainer
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
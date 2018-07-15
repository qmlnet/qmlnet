using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;

namespace Qt.NetCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public IntPtr IsTypeValid;
        public IntPtr BuildTypeInfo;
        public IntPtr ReleaseGCHandle;
        public IntPtr InstantiateType;
        public IntPtr ReadProperty;
        public IntPtr WriteProperty;
    }

    public interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPWStr)]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseGCHandle")]
        void ReleaseGCHandle(IntPtr handle);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_buildTypeInfo")]
        void BuildTypeInfo(IntPtr handle);

        [NativeSymbol(Entrypoint = "type_info_callbacks_instantiateType")]
        IntPtr InstantiateType([MarshalAs(UnmanagedType.LPWStr)]string typeName);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_readProperty")]
        void ReadProperty(IntPtr property, IntPtr target, IntPtr result);

        [NativeSymbol(Entrypoint = "type_info_callbacks_writeProperty")]
        void WriteProperty(IntPtr property, IntPtr target, IntPtr value);
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseGCHandle(IntPtr handle);
        
        void BuildTypeInfo(NetTypeInfo typeInfo);

        GCHandle InstantiateType(string typeName);

        void ReadProperty(NetPropertyInfo property, NetInstance target, NetVariant result);

        void WriteProperty(NetPropertyInfo property, NetInstance target, NetVariant value);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        BuildTypeInfoDelegate _buildTypeInfoDelegate;
        ReleaseGCHandleDelegate _releaseGCHandleDelegate;
        InstantiateTypeDelgate _instantiateTypeDelgate;
        ReadPropertyDelegate _readPropertyDelegate;
        WritePropertyDelegate _writePropertyDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPWStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void BuildTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseGCHandleDelegate(IntPtr handle);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr InstantiateTypeDelgate([MarshalAs(UnmanagedType.LPWStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReadPropertyDelegate(IntPtr property, IntPtr target, IntPtr result);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WritePropertyDelegate(IntPtr property, IntPtr target, IntPtr value);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);

            _releaseGCHandleDelegate = ReleaseGCHandle;
            GCHandle.Alloc(_releaseGCHandleDelegate);
            
            _buildTypeInfoDelegate = BuildTypeInfo;
            GCHandle.Alloc(_buildTypeInfoDelegate);
            
            _instantiateTypeDelgate = InstantiateType;
            GCHandle.Alloc(_instantiateTypeDelgate);

            _readPropertyDelegate = ReadProperty;
            GCHandle.Alloc(_readPropertyDelegate);

            _writePropertyDelegate = WriteProperty;
            GCHandle.Alloc(_writePropertyDelegate);
        }

        private bool IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName);
        }
        
        private void ReleaseGCHandle(IntPtr handle)
        {
            _callbacks.ReleaseGCHandle(handle);
        }

        private void BuildTypeInfo(IntPtr typeInfo)
        {
            _callbacks.BuildTypeInfo(new NetTypeInfo(typeInfo));
        }
        
        private IntPtr InstantiateType(string typeName)
        {
            return GCHandle.ToIntPtr(_callbacks.InstantiateType(typeName));
        }

        private void ReadProperty(IntPtr p, IntPtr t, IntPtr r)
        {
            using (var property = new NetPropertyInfo(p))
            {
                using (var target = new NetInstance(t))
                {
                    using (var result = new NetVariant(r))
                    {
                        _callbacks.ReadProperty(property, target, result);
                    }
                }
            }
        }

        private void WriteProperty(IntPtr p, IntPtr t, IntPtr v)
        {
            using (var property = new NetPropertyInfo(p))
            {
                using (var target = new NetInstance(t))
                {
                    using (var value = new NetVariant(v))
                    {
                        _callbacks.WriteProperty(property, target, value);
                    }
                }
            }
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                BuildTypeInfo = Marshal.GetFunctionPointerForDelegate(_buildTypeInfoDelegate),
                ReleaseGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseGCHandleDelegate),
                InstantiateType = Marshal.GetFunctionPointerForDelegate(_instantiateTypeDelgate),
                ReadProperty = Marshal.GetFunctionPointerForDelegate(_readPropertyDelegate),
                WriteProperty = Marshal.GetFunctionPointerForDelegate(_writePropertyDelegate)
            };
        }
    }
}
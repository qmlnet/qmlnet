using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
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
    }

    public interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPStr)]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseGCHandle")]
        void ReleaseGCHandle(IntPtr handle);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_buildTypeInfo")]
        void BuildTypeInfo(IntPtr handle);

        [NativeSymbol(Entrypoint = "type_info_callbacks_instantiateType")]
        IntPtr InstantiateType(IntPtr type);
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseGCHandle(IntPtr handle);
        
        void BuildTypeInfo(NetTypeInfo typeInfo);

        NetInstance InstantiateType(NetTypeInfo typeInfo);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        BuildTypeInfoDelegate _buildTypeInfoDelegate;
        ReleaseGCHandleDelegate _releaseGCHandleDelegate;
        InstantiateTypeDelgate _instantiateTypeDelgate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void BuildTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseGCHandleDelegate(IntPtr handle);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr InstantiateTypeDelgate(IntPtr typeInfo);
        
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
        
        private IntPtr InstantiateType(IntPtr typeInfo)
        {
            var instance = _callbacks.InstantiateType(new NetTypeInfo(typeInfo));
            return instance?.Handle ?? IntPtr.Zero;
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                BuildTypeInfo = Marshal.GetFunctionPointerForDelegate(_buildTypeInfoDelegate),
                ReleaseGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseGCHandleDelegate),
                InstantiateType = Marshal.GetFunctionPointerForDelegate(_instantiateTypeDelgate)
            };
        }
    }
}
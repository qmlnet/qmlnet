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
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseGCHandle(IntPtr handle);
        
        void BuildTypeInfo(NetTypeInfo typeInfo);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        BuildTypeInfoDelegate _buildTypeInfoDelegate;
        ReleaseGCHandleDelegate _releaseGCHandleDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void BuildTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseGCHandleDelegate(IntPtr handle);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);

            _releaseGCHandleDelegate = ReleaseGCHandle;
            GCHandle.Alloc(_releaseGCHandleDelegate);
            
            _buildTypeInfoDelegate = BuildTypeInfo;
            GCHandle.Alloc(_buildTypeInfoDelegate);
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

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                BuildTypeInfo = Marshal.GetFunctionPointerForDelegate(_buildTypeInfoDelegate),
                ReleaseGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseGCHandleDelegate)
            };
        }
    }
}
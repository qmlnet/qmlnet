using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qt.NetCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public IntPtr IsTypeValid;
        public IntPtr BuildTypeInfo;
    }

    public interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPStr)]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_buildTypeInfo")]
        void BuildTypeInfo(IntPtr handle);
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);
        
        void BuildTypeInfo(NetTypeInfo typeInfo);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        BuildTypeInfoDelegate _buildTypeInfoDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void BuildTypeInfoDelegate(IntPtr typeInfo);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);
            
            _buildTypeInfoDelegate = BuildTypeInfo;
            GCHandle.Alloc(_buildTypeInfoDelegate);
        }

        private bool IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName);
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
                BuildTypeInfo = Marshal.GetFunctionPointerForDelegate(_buildTypeInfoDelegate)
            };
        }
    }
}
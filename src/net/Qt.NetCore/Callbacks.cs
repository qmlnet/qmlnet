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
    }

    public interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_manager_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_manager_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPStr)]string typeName);
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPStr)]string typeName);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);
        }

        private bool IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName);
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate)
            };
        }
    }
}
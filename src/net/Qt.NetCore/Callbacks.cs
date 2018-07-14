using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Qt.NetCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public IntPtr IsTypeValid;
    }

    public interface ICallbacksIterop
    {
        void registerCallbacks(ref Callbacks callbacks);

        bool isTypeValid();
    }

    public interface ICallbacks
    {
        bool IsTypeValid();
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate();
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);
        }

        private bool IsTypeValid()
        {
            return _callbacks.IsTypeValid();
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
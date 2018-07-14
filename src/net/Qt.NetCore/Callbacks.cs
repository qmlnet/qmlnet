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

    public interface ICallbacks
    {
        void registerCallbacks();
    }

    public class CallbacksImpl
    {
        private IsTypeValidDelegate _isTypeValidDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate();
        
        public CallbacksImpl()
        {
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);
        }
        
        public bool IsTypeValid()
        {
            return true;
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
using System;

namespace Qml.Net.Internal.Qml
{
    internal class NetQObjectSignalConnection : BaseDisposable
    {
        public NetQObjectSignalConnection(IntPtr handle, bool ownsHandle = true) 
            : base(handle, ownsHandle)
        {
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetQObjectSignalConnection.Destroy(ptr);
        }
    }
    
    internal class NetQObjectSignalConnectionInterop
    {
        [NativeSymbol(Entrypoint = "net_qobject_signal_handler_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr signalHandler);
    }
}
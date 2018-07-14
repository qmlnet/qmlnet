using System;

namespace Qt.NetCore
{
    public abstract class BaseDisposable : IDisposable
    {
        IntPtr _handle;
        bool _disposed = false;
        readonly bool _ownesHandle;

        protected BaseDisposable(IntPtr handle, bool ownsHandle = true)
        {
            _handle = handle;
            _ownesHandle = ownsHandle;
        }
        
        ~BaseDisposable()
        {
            Dispose(false);
        }

        public IntPtr Handle
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException($"Type {GetType().Name} is disposed.");
                }
                
                return _handle;
            }
        }

        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                DisposeManaged();
            }

            if (_ownesHandle)
            {
                DisposeUnmanaged(_handle);
            }

            _handle = IntPtr.Zero;

            _disposed = true;
        }

        protected virtual void DisposeManaged()
        {
            
        }

        protected abstract void DisposeUnmanaged(IntPtr ptr);
    }
}
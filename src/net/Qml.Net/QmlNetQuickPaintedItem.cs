using System;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;
using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public abstract class QmlNetQuickPaintedItem
    {
        private IntPtr _ref;

        public int Height => _ref != IntPtr.Zero ? Interop.QmlNetPaintedItem.GetHeight(_ref) : 0;

        public int Width => _ref != IntPtr.Zero ? Interop.QmlNetPaintedItem.GetWidth(_ref) : 0;

        public QmlNetQuickPaintedItem()
        {
            if (_callbacks == null)
            {
                _callbacks = new QmlNetPaintedItemCallbacksImpl();
            }
        }
        
        internal void SetRef(IntPtr @ref)
        {
            _ref = @ref;
        }

        public void Update()
        {
            if (_ref != IntPtr.Zero)
            {
                Interop.QmlNetPaintedItem.Update(_ref);
            }
        }

        public abstract void Paint(INetQPainter painter);

        public virtual void OnHeightChanged(int height)
        {
        }

        public virtual void OnWidthChanged(int width)
        {
        }

        private static QmlNetPaintedItemCallbacksImpl _callbacks;
    }
    
    internal class QmlNetPaintedItemCallbacksImpl
    {
        internal QmlNetPaintedItemCallbacksImpl()
        {
            _setRef = SetRef;
            GCHandle.Alloc(_setRef);
            _paint = Paint;
            GCHandle.Alloc(_paint);
            _heightChanged = HeightChanged;
            GCHandle.Alloc(_heightChanged);
            _widthChanged = WidthChanged;
            GCHandle.Alloc(_paint);

            var cb = Callbacks();

            Interop.QmlNetPaintedItem.SetCallbacks(ref cb);
        }

        internal QmlNetPaintedItemCallbacks Callbacks()
        {
            return new QmlNetPaintedItemCallbacks
            {
                SetRef = Marshal.GetFunctionPointerForDelegate(_setRef),
                Paint = Marshal.GetFunctionPointerForDelegate(_paint),
                HeightChanged = Marshal.GetFunctionPointerForDelegate(_heightChanged),
                WidthChanged = Marshal.GetFunctionPointerForDelegate(_widthChanged)
            };
        }

        SetRefDel _setRef;
        PaintDel _paint;
        HeightChangedDel _heightChanged;
        WidthChangedDel _widthChanged;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void SetRefDel(ulong objectId, IntPtr @ref);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void PaintDel(ulong objectId, IntPtr netQPainter);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void HeightChangedDel(ulong objectId, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void WidthChangedDel(ulong objectId, int height);

        internal void SetRef(ulong objectId, IntPtr @ref)
        {
            object obj = null;
            if (ObjectIdReferenceTracker.TryGetObjectFor(objectId, out obj))
            {
                (obj as QmlNetQuickPaintedItem)?.SetRef(@ref);
            }
        }

        internal void Paint(ulong objectId, IntPtr netQPainter)
        {
            object obj = null;
            if (ObjectIdReferenceTracker.TryGetObjectFor(objectId, out obj))
            {
                (obj as QmlNetQuickPaintedItem)?.Paint(new INetQPainter(netQPainter));
            }
        }

        internal void HeightChanged(ulong objectId, int height)
        {
            object obj = null;
            if (ObjectIdReferenceTracker.TryGetObjectFor(objectId, out obj))
            {
                (obj as QmlNetQuickPaintedItem)?.OnHeightChanged(height);
            }
        }

        internal void WidthChanged(ulong objectId, int width)
        {
            object obj = null;
            if (ObjectIdReferenceTracker.TryGetObjectFor(objectId, out obj))
            {
                (obj as QmlNetQuickPaintedItem)?.OnWidthChanged(width);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct QmlNetPaintedItemCallbacks
    {
        public IntPtr SetRef;
        public IntPtr Paint;
        public IntPtr HeightChanged;
        public IntPtr WidthChanged;
    }
    
    internal class QmlNetPaintedItemInterop
    {
        [NativeSymbol(Entrypoint = "qqmlnetpainteditembase_setCallbacks")]
        public SetCallbacksDel SetCallbacks { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetCallbacksDel(ref QmlNetPaintedItemCallbacks callbacks);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditembase_update")]
        public UpdateDel Update { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateDel(IntPtr paintedItem);
        
        [NativeSymbol(Entrypoint = "qqmlnetpainteditembase_getHeight")]
        public GetHeightDel GetHeight { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetHeightDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditembase_getWidth")]
        public GetWidthDel GetWidth { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetWidthDel(IntPtr paintedItem);
    }
}
using System;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QModelIndex : BaseDisposable
    {
        internal QModelIndex(IntPtr handle, bool ownsHandle = true) 
            : base(handle, ownsHandle)
        {
        }
        public static QModelIndex BlankIndex() {
            return new QModelIndex(Interop.NetQModelIndex.BlankModelIndex(), true);
        }
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetQModelIndex.Destroy(ptr);
        }
        public int Row {
            get {
                return Interop.NetQModelIndex.Column(Handle);
            }
        }
        public int Column {
            get {
                return Interop.NetQModelIndex.Row(Handle);
            }
        }
        public QModelIndex Parent {
            get {
                return new QModelIndex(Interop.NetQModelIndex.Parent(Handle));
            }
        }
    }
    internal class NetQModelIndexInterop
    {
        [NativeSymbol(Entrypoint = "net_qmodelindex_destroy")]
        public DestroyDel Destroy { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DestroyDel(IntPtr qModelIndex);


        [NativeSymbol(Entrypoint = "net_qmodelindex_row")]
        public RowDel Row { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RowDel(IntPtr qModelIndex);

        [NativeSymbol(Entrypoint = "net_qmodelindex_column")]
        public ColumnDel Column { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ColumnDel(IntPtr qModelIndex);

        [NativeSymbol(Entrypoint = "net_qmodelindex_parent")]
        public ParentDel Parent { get; set; }

        [NativeSymbol(Entrypoint = "net_qmodelindex_create")]
        public NetAbstractItemModelInterop.CreateDel BlankModelIndex { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ParentDel(IntPtr qModelIndex);
    }
}
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;
using System.Collections.Generic;

namespace Qml.Net
{
    internal static class ReflectionExtensions {
        public static bool IsOverride(this MethodInfo m) {
            return m.GetBaseDefinition().DeclaringType != m.DeclaringType;
        }
        public static bool MemberIsOverride(this Type t, string name) {
            try {
                return ((MethodInfo)t.GetMember("Flags")[0]).IsOverride();
            } catch {
                return false;
            }
        }
    }
    public enum ItemDataRole {
        DisplayRole = 0,
        DecorationRole = 1,
        EditRole = 2,
        ToolTipRole = 3,
        StatusTipRole = 4,
        WhatsThisRole = 5
    }
    public class QAbstractItemModel : BaseDisposable
    {
        public QAbstractItemModel()
            : base(Interop.NetAbstractItemModel.Create(), true)
        {
            var type = this.GetType();
            if (type.MemberIsOverride("Flags")) {
                flagsDel = (ptr) => {
                    return Flags(new QModelIndex(ptr, true));
                };
                flagsDelPtr = Marshal.GetFunctionPointerForDelegate(flagsDel);
                Interop.NetAbstractItemModel.SetFlags(Handle, flagsDelPtr);
            } else {
                flagsDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.FlagsDelegate>(Interop.NetAbstractItemModel.GetFlags(Handle));
            }
            if (type.MemberIsOverride("Data")) {
                dataDel = (ptr, role) => {
                    var variant = new Internal.Qml.NetVariant();
                    var ret = Data(new QModelIndex(ptr, true), role);
                    Helpers.PackValue(ret, variant);
                    return variant.Handle;
                };
                dataDelPtr = Marshal.GetFunctionPointerForDelegate(dataDel);
                Interop.NetAbstractItemModel.SetData(Handle, dataDelPtr);
            } else {
                dataDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.DataDelegate>(Interop.NetAbstractItemModel.GetData(Handle));
            }
            if (type.MemberIsOverride("HeaderData")) {
                headerDataDel = (section, orientation, role) => {
                    var variant = new Internal.Qml.NetVariant();
                    var ret = HeaderData(section, orientation, role);
                    Helpers.PackValue(ret, variant);
                    return variant.Handle;
                };
                headerDataDelPtr = Marshal.GetFunctionPointerForDelegate(headerDataDel);
                Interop.NetAbstractItemModel.SetData(Handle, dataDelPtr);
            } else {
                headerDataDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.HeaderDataDelegate>(Interop.NetAbstractItemModel.GetHeaderData(Handle));
            }
            if (type.MemberIsOverride("RowCount")) {
                rowCountDel = (idx) => {
                    return RowCount(new QModelIndex(idx, true));
                };
                rowCountDelPtr = Marshal.GetFunctionPointerForDelegate(rowCountDel);
                Interop.NetAbstractItemModel.SetRowCount(Handle, rowCountDelPtr);
            } else {
                rowCountDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.RowCountDelegate>(Interop.NetAbstractItemModel.GetRowCount(Handle));
            }
            if (type.MemberIsOverride("ColumnCount")) {
                columnCountDel = (idx) => {
                    return RowCount(new QModelIndex(idx, true));
                };
                columnCountDelPtr = Marshal.GetFunctionPointerForDelegate(columnCountDel);
                Interop.NetAbstractItemModel.SetColumnCount(Handle, columnCountDelPtr);
            } else {
                columnCountDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.ColumnCountDelegate>(Interop.NetAbstractItemModel.GetColumnCount(Handle));
            }
            if (type.MemberIsOverride("Index")) {
                indexDel = (row, col, ptr) => {
                    return Index(row, col, new QModelIndex(ptr, true)).Handle;
                };
                indexDelPtr = Marshal.GetFunctionPointerForDelegate(indexDel);
                Interop.NetAbstractItemModel.SetIndex(Handle, indexDelPtr);
            } else {
                indexDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.IndexDelegate>(Interop.NetAbstractItemModel.GetColumnCount(Handle));
            }
            if (type.MemberIsOverride("Parent")) {
                parentDel = (child) => {
                    return Parent(new QModelIndex(child, true)).Handle;
                };
                parentDelPtr = Marshal.GetFunctionPointerForDelegate(parentDel);
                Interop.NetAbstractItemModel.SetParent(Handle, parentDelPtr);
            } else {
                parentDel = Marshal.GetDelegateForFunctionPointer<NetAbstractItemModelInterop.ParentDelegate>(Interop.NetAbstractItemModel.GetParent(Handle));
            }
            if (type.MemberIsOverride("RoleNames")) {
                roleNamesDel = () => {
                    var ret = RoleNames();
                    var hash = Interop.NetAbstractItemModel.CreateHash();
                    foreach (var item in ret)
                    {
                        Interop.NetAbstractItemModel.InsertIntoHash(hash, item.Key, item.Value);
                    }
                    return hash;
                };
                roleNamesDelPtr = Marshal.GetFunctionPointerForDelegate(roleNamesDel);
                Interop.NetAbstractItemModel.SetRoleNames(Handle, roleNamesDelPtr);
            }
        }
        /// Flags for a given QModelIndex.
        public virtual int Flags(QModelIndex index) {
            return flagsDel(index.Handle);
        }
        /// The data for the given QModelIndex and role.
        public virtual object Data(QModelIndex index, int role) {
            var obj = dataDel(index.Handle, role);
            var variant = new Internal.Qml.NetVariant(obj, true);
            return variant.AsObject();
        }
        /// The data for the header in a specific section and orientation and role.
        public virtual object HeaderData(int section, int orientation, int role) {
            var obj = headerDataDel(section, orientation, role);
            var variant = new Internal.Qml.NetVariant(obj, true);
            return variant.AsObject();
        }
        /// How many rows the model has under the given parent.
        public virtual int RowCount(QModelIndex parent) {
            return rowCountDel(parent.Handle);
        }
        /// How many columns the model has.
        public virtual int ColumnCount(QModelIndex parent) {
            return columnCountDel(parent.Handle);
        }
        /// Creates a QModelIndex for the given row + column + parent index.
        public virtual QModelIndex Index(int row, int column, QModelIndex parent) {
            return new QModelIndex(indexDel(row, column, parent.Handle), true);
        }
        /// Gets the parent index of a child index.
        public virtual QModelIndex Parent(QModelIndex child) {
            return new QModelIndex(parentDel(child.Handle), true);
        }
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetAbstractItemModel.Destroy(ptr);
        }
        /// Signals the start of inserting columns into the model.
        /// You must call this function before inserting more columns into the model.
        protected void BeginInsertColumns(QModelIndex parent, int from, int to) {
            Interop.NetAbstractItemModel.BeginInsertColumns(Handle, parent.Handle, from, to);
        }
        /// Signals the end of inserting columns into the model.
        protected void EndInsertColumns() {
            Interop.NetAbstractItemModel.EndInsertColumns(Handle);
        }
        /// Signals the start of removing columns from the model.
        /// You must call this function before removing columns into the model.
        protected void BeginRemoveColumns(QModelIndex parent, int from, int to) {
            Interop.NetAbstractItemModel.BeginRemoveColumns(Handle, parent.Handle, from, to);
        }
        /// Signals the end of removing columns from the model.
        protected void EndRemoveColumns() {
            Interop.NetAbstractItemModel.EndRemoveColumns(Handle);
        }
        /// Signals the start of moving columns in the model.
        /// You must call this function before moving columns in the model.
        protected void BeginMoveColumns(QModelIndex sourceParent, int sourceFirst, int sourceLast, QModelIndex destinationParent, int destinationChild) {
            Interop.NetAbstractItemModel.BeginMoveColumns(Handle, sourceParent.Handle, sourceFirst, sourceLast, destinationParent.Handle, destinationChild);
        }
        /// Signals the end of moving columns in the model.
        protected void EndMoveColumns() {
            Interop.NetAbstractItemModel.EndMoveColumns(Handle);
        }
        /// Signals the start of inserting rows into the model.
        /// You must call this function before inserting more rows into the model.
        protected void BeginInsertRows(QModelIndex parent, int from, int to) {
            Interop.NetAbstractItemModel.BeginInsertRows(Handle, parent.Handle, from, to);
        }
        /// Signals the end of inserting rows into the model.
        protected void EndInsertRows() {
            Interop.NetAbstractItemModel.EndInsertRows(Handle);
        }
        /// Signals the start of removing rows from the model.
        /// You must call this function before removing rows into the model.
        protected void BeginRemoveRows(QModelIndex parent, int from, int to) {
            Interop.NetAbstractItemModel.BeginRemoveRows(Handle, parent.Handle, from, to);
        }
        /// Signals the end of removing rows from the model.
        protected void EndRemoveRows() {
            Interop.NetAbstractItemModel.EndRemoveRows(Handle);
        }
        /// Signals the start of moving rows in the model.
        /// You must call this function before moving rows in the model.
        protected void BeginMoveRows(QModelIndex sourceParent, int sourceFirst, int sourceLast, QModelIndex destinationParent, int destinationChild) {
            Interop.NetAbstractItemModel.BeginMoveRows(Handle, sourceParent.Handle, sourceFirst, sourceLast, destinationParent.Handle, destinationChild);
        }
        /// Signals the end of moving rows in the model.
        protected void EndMoveRows() {
            Interop.NetAbstractItemModel.EndMoveRows(Handle);
        }
        /// Signals the start of resetting the model.
        /// You must call this function before resetting the model's state.
        protected void BeginResetModel() {
            Interop.NetAbstractItemModel.BeginResetModel(Handle);
        }
        /// Signals the end of resetting the model.
        protected void EndResetModel() {
            Interop.NetAbstractItemModel.EndResetModel(Handle);
        }
        /// The mapping of strings to role IDs.
        /// This is used to indicate the QML names of properties in
        /// components such as the ListView and Repeater.
        protected virtual Dictionary<int,string> RoleNames() {
            return new Dictionary<int, string>(){
                {(int)ItemDataRole.DisplayRole, "display"},
                {(int)ItemDataRole.DecorationRole, "decoration"},
                {(int)ItemDataRole.EditRole, "edit"},
                {(int)ItemDataRole.ToolTipRole, "toolTip"},
                {(int)ItemDataRole.StatusTipRole, "statusTip"},
                {(int)ItemDataRole.WhatsThisRole, "whatsThis"},
            };
        }
        private NetAbstractItemModelInterop.FlagsDelegate flagsDel;
        private IntPtr flagsDelPtr;
        private NetAbstractItemModelInterop.DataDelegate dataDel;
        private IntPtr dataDelPtr;
        private NetAbstractItemModelInterop.HeaderDataDelegate headerDataDel;
        private IntPtr headerDataDelPtr;
        private NetAbstractItemModelInterop.RowCountDelegate rowCountDel;
        private IntPtr rowCountDelPtr;
        private NetAbstractItemModelInterop.ColumnCountDelegate columnCountDel;
        private IntPtr columnCountDelPtr;
        private NetAbstractItemModelInterop.IndexDelegate indexDel;
        private IntPtr indexDelPtr;
        private NetAbstractItemModelInterop.ParentDelegate parentDel;
        private IntPtr parentDelPtr;
        private NetAbstractItemModelInterop.RoleNamesDelegate roleNamesDel;
        private IntPtr roleNamesDelPtr;
    }
    internal class NetAbstractItemModelInterop
    {
        //
        // Pointer Types
        //
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int FlagsDelegate( IntPtr idx );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr DataDelegate( IntPtr idx, int role );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr HeaderDataDelegate( int section, int orientation, int role );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RowCountDelegate( IntPtr parentIdx );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ColumnCountDelegate( IntPtr parentIdx );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr IndexDelegate( int row, int col, IntPtr parent );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ParentDelegate( IntPtr child );
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr RoleNamesDelegate();
        //
        // Instance Functions
        //
        [NativeSymbol(Entrypoint = "net_abstract_item_model_destroy")]
        public DestroyDel Destroy { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DestroyDel(IntPtr model);

        [NativeSymbol(Entrypoint = "net_abstract_item_model_create")]
        public CreateDel Create { get; set; }
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr CreateDel();

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr SetFuncDel(IntPtr instance, IntPtr func);
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetFuncDel(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_flags")]
        public SetFuncDel SetFlags {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_flags")]
        public GetFuncDel GetFlags { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_data")]
        public SetFuncDel SetData {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_data")]
        public GetFuncDel GetData { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_header_data")]
        public SetFuncDel SetHeaderData {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_header_data")]
        public GetFuncDel GetHeaderData { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_row_count")]
        public SetFuncDel SetRowCount {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_row_count")]
        public GetFuncDel GetRowCount { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_column_count")]
        public SetFuncDel SetColumnCount {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_column_count")]
        public GetFuncDel GetColumnCount { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_index")]
        public SetFuncDel SetIndex {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_index")]
        public GetFuncDel GetIndex { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_parent")]
        public SetFuncDel SetParent {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_parent")]
        public GetFuncDel GetParent { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_set_role_names")]
        public SetFuncDel SetRoleNames {get; set;}

        [NativeSymbol(Entrypoint = "net_abstract_item_model_get_role_names")]
        public GetFuncDel GetRoleNames { get; set; }

        //
        // Protected Functions
        //
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SideEffectDelegate( IntPtr instance );

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endInsertColumns")]
        public SideEffectDelegate EndInsertColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endInsertRows")]
        public SideEffectDelegate EndInsertRows { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endMoveColumns")]
        public SideEffectDelegate EndMoveColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endMoveRows")]
        public SideEffectDelegate EndMoveRows { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endRemoveColumns")]
        public SideEffectDelegate EndRemoveColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endRemoveRows")]
        public SideEffectDelegate EndRemoveRows { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginResetModel")]
        public SideEffectDelegate BeginResetModel { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_endResetModel")]
        public SideEffectDelegate EndResetModel { get; set; }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BeginInsertOrRemoveDelegate( IntPtr instance, IntPtr parent, int first, int last );

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginInsertColumns")]
        public BeginInsertOrRemoveDelegate BeginInsertColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginInsertRows")]
        public BeginInsertOrRemoveDelegate BeginInsertRows { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginRemoveColumns")]
        public BeginInsertOrRemoveDelegate BeginRemoveColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginRemoveRows")]
        public BeginInsertOrRemoveDelegate BeginRemoveRows { get; set; }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BeginMoveDelegate( IntPtr instance, IntPtr sourceParent, int sourceFirst, int sourceLast, IntPtr destinationParent, int destinationChild );

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginMoveColumns")]
        public BeginMoveDelegate BeginMoveColumns { get; set; }

        [NativeSymbol(Entrypoint = "net_abstract_item_model_beginMoveRows")]
        public BeginMoveDelegate BeginMoveRows { get; set; }

        //
        // QHash
        //

        [NativeSymbol(Entrypoint = "net_qmodelhash_destroy")]
        public DestroyDel DestroyHash { get; set; }

        [NativeSymbol(Entrypoint = "net_qmodelhash_create")]
        public CreateDel CreateHash { get; set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void HashInsertDel(IntPtr hashMap, int num, string data);

        [NativeSymbol(Entrypoint = "net_qmodelhash_insert")]
        public HashInsertDel InsertIntoHash { get; set; }
    }
}
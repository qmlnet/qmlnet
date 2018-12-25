using System;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Types
{
    internal class NetTypeInfo : BaseDisposable
    {
        public NetTypeInfo(string fullTypeName)
            : this(Interop.NetTypeInfo.Create(fullTypeName))
        {
        }

        public NetTypeInfo(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
        }

        public string FullTypeName => Utilities.ContainerToString(Interop.NetTypeInfo.GetFullTypeName(Handle));

        public string ClassName
        {
            get => Utilities.ContainerToString(Interop.NetTypeInfo.GetClassName(Handle));
            set => Interop.NetTypeInfo.SetClassName(Handle, value);
        }

        public NetVariantType PrefVariantType
        {
            get => Interop.NetTypeInfo.GetPrefVariantType(Handle);
            set => Interop.NetTypeInfo.SetPrefVariantType(Handle, value);
        }

        public bool IsArray
        {
            get => Interop.NetTypeInfo.GetIsArray(Handle);
            set => Interop.NetTypeInfo.SetIsArray(Handle, value);
        }

        public bool IsList
        {
            get => Interop.NetTypeInfo.GetIsList(Handle);
            set => Interop.NetTypeInfo.SetIsList(Handle, value);
        }

        public void AddMethod(NetMethodInfo methodInfo)
        {
            Interop.NetTypeInfo.AddMethod(Handle, methodInfo.Handle);
        }

        public int MethodCount => Interop.NetTypeInfo.GetMethodCount(Handle);

        public NetMethodInfo GetMethod(int index)
        {
            var result = Interop.NetTypeInfo.GetMethodInfo(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfo(result);
        }

        public int LocalMethodCount => Interop.NetTypeInfo.GetLocalMethodCount(Handle);

        public NetMethodInfo GetLocalMethod(int index)
        {
            var result = Interop.NetTypeInfo.GetLocalMethodInfo(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfo(result);
        }

        public int StaticMethodCount => Interop.NetTypeInfo.GetStaticMethodCount(Handle);

        public NetMethodInfo GetStaticMethod(int index)
        {
            var result = Interop.NetTypeInfo.GetStaticMethodInfo(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfo(result);
        }

        public void AddProperty(NetPropertyInfo property)
        {
            Interop.NetTypeInfo.AddProperty(Handle, property.Handle);
        }

        public int PropertyCount => Interop.NetTypeInfo.GetPropertyCount(Handle);

        public NetPropertyInfo GetProperty(int index)
        {
            var result = Interop.NetTypeInfo.GetProperty(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetPropertyInfo(result);
        }

        public void AddSignal(NetSignalInfo signal)
        {
            Interop.NetTypeInfo.AddSignal(Handle, signal.Handle);
        }

        public int SignalCount => Interop.NetTypeInfo.GetSignalCount(Handle);

        public NetSignalInfo GetSignal(int index)
        {
            var result = Interop.NetTypeInfo.GetSignal(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetSignalInfo(result);
        }

        public bool IsLoaded => Interop.NetTypeInfo.IsLoaded(Handle);

        public bool IsLoading => Interop.NetTypeInfo.IsLoading(Handle);

        public void EnsureLoaded()
        {
            Interop.NetTypeInfo.EnsureLoaded(Handle);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetTypeInfo.Destroy(ptr);
        }
    }

    internal class NetTypeInfoInterop
    {
        [NativeSymbol(Entrypoint = "type_info_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel([MarshalAs(UnmanagedType.LPWStr)]string fullTypeName);

        [NativeSymbol(Entrypoint = "type_info_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_getFullTypeName")]
        public GetFullTypeNameDel GetFullTypeName { get; set; }

        public delegate IntPtr GetFullTypeNameDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_setClassName")]
        public SetClassNameDel SetClassName { get; set; }

        public delegate void SetClassNameDel(IntPtr netTypeInfo, [MarshalAs(UnmanagedType.LPWStr)]string className);

        [NativeSymbol(Entrypoint = "type_info_getClassName")]
        public GetClassNameDel GetClassName { get; set; }

        public delegate IntPtr GetClassNameDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_setPrefVariantType")]
        public SetPrefVariantTypeDel SetPrefVariantType { get; set; }

        public delegate void SetPrefVariantTypeDel(IntPtr netTypeInfo, NetVariantType variantType);

        [NativeSymbol(Entrypoint = "type_info_getPrefVariantType")]
        public GetPrefVariantTypeDel GetPrefVariantType { get; set; }

        public delegate NetVariantType GetPrefVariantTypeDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_setIsArray")]
        public GetIsArrayDel GetIsArray { get; set; }

        public delegate bool GetIsArrayDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_getIsArray")]
        public SetIsArrayDel SetIsArray { get; set; }

        public delegate void SetIsArrayDel(IntPtr netTypeInfo, bool isArray);

        [NativeSymbol(Entrypoint = "type_info_setIsList")]
        public GetIsListDel GetIsList { get; set; }

        public delegate bool GetIsListDel(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_getIsList")]
        public SetIsListDel SetIsList { get; set; }

        public delegate void SetIsListDel(IntPtr netTypeInfo, bool isList);

        [NativeSymbol(Entrypoint = "type_info_addMethod")]
        public AddMethodDel AddMethod { get; set; }

        public delegate void AddMethodDel(IntPtr typeInfo, IntPtr methodInfo);

        [NativeSymbol(Entrypoint = "type_info_getMethodCount")]
        public GetMethodCountDel GetMethodCount { get; set; }

        public delegate int GetMethodCountDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_getMethodInfo")]
        public GetMethodInfoDel GetMethodInfo { get; set; }

        public delegate IntPtr GetMethodInfoDel(IntPtr typeInfo, int index);

        [NativeSymbol(Entrypoint = "type_info_getLocalMethodCount")]
        public GetLocalMethodCountDel GetLocalMethodCount { get; set; }

        public delegate int GetLocalMethodCountDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_getLocalMethodInfo")]
        public GetLocalMethodInfoDel GetLocalMethodInfo { get; set; }

        public delegate IntPtr GetLocalMethodInfoDel(IntPtr typeInfo, int index);

        [NativeSymbol(Entrypoint = "type_info_getStaticMethodCount")]
        public GetStaticMethodCountDel GetStaticMethodCount { get; set; }

        public delegate int GetStaticMethodCountDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_getStaticMethodInfo")]
        public GetStaticMethodInfoDel GetStaticMethodInfo { get; set; }

        public delegate IntPtr GetStaticMethodInfoDel(IntPtr typeInfo, int index);

        [NativeSymbol(Entrypoint = "type_info_addProperty")]
        public AddPropertyDel AddProperty { get; set; }

        public delegate void AddPropertyDel(IntPtr typeInfo, IntPtr property);

        [NativeSymbol(Entrypoint = "type_info_getPropertyCount")]
        public GetPropertyCountDel GetPropertyCount { get; set; }

        public delegate int GetPropertyCountDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_getProperty")]
        public GetPropertyDel GetProperty { get; set; }

        public delegate IntPtr GetPropertyDel(IntPtr typeInfo, int index);

        [NativeSymbol(Entrypoint = "type_info_addSignal")]
        public AddSignalDel AddSignal { get; set; }

        public delegate void AddSignalDel(IntPtr typeInfo, IntPtr signal);

        [NativeSymbol(Entrypoint = "type_info_getSignalCount")]
        public GetSignalCountDel GetSignalCount { get; set; }

        public delegate int GetSignalCountDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_getSignal")]
        public GetSignalDel GetSignal { get; set; }

        public delegate IntPtr GetSignalDel(IntPtr typeInfo, int index);

        [NativeSymbol(Entrypoint = "type_info_isLoaded")]
        public IsLoadedDel IsLoaded { get; set; }

        public delegate bool IsLoadedDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_isLoading")]
        public IsLoadingDel IsLoading { get; set; }

        public delegate bool IsLoadingDel(IntPtr typeInfo);

        [NativeSymbol(Entrypoint = "type_info_ensureLoaded")]
        public EnsureLoadedDel EnsureLoaded { get; set; }

        public delegate void EnsureLoadedDel(IntPtr typeInfo);
    }
}
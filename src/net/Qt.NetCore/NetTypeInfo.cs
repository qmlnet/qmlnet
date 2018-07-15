using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore
{
    public class NetTypeInfo : BaseDisposable
    {
        public NetTypeInfo(string fullTypeName)
            :this(Interop.NetTypeInfo.Create(fullTypeName))
        {
        }

        public NetTypeInfo(IntPtr handle, bool ownsHandle = true)
            :base(handle, ownsHandle)
        {
            
        }

        public string FullTypeName => Interop.NetTypeInfo.GetFullTypeName(Handle);

        public string ClassName
        {
            get => Interop.NetTypeInfo.GetClassName(Handle);
            set => Interop.NetTypeInfo.SetClassName(Handle, value);
        }

        public NetVariantType PrefVariantType
        {
            get => Interop.NetTypeInfo.GetPrefVariantType(Handle);
            set => Interop.NetTypeInfo.SetPrefVariantType(Handle, value);
        }

        public void AddMethod(NetMethodInfo methodInfo)
        {
            Interop.NetTypeInfo.AddMethod(Handle, methodInfo.Handle);
        }

        public uint MethodCount => Interop.NetTypeInfo.GetMethodCount(Handle);

        public NetMethodInfo GetMethod(uint index)
        {
            var result = Interop.NetTypeInfo.GetMethodInfo(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfo(result);
        }
        
        public void AddProperty(NetPropertyInfo property)
        {
            Interop.NetTypeInfo.AddProperty(Handle, property.Handle);
        }

        public uint PropertyCount => Interop.NetTypeInfo.GetPropertyCount(Handle);

        public NetPropertyInfo GetProperty(uint index)
        {
            var result = Interop.NetTypeInfo.GetProperty(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetPropertyInfo(result);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetTypeInfo.Destroy(ptr);
        }
    }

    public interface INetTypeInfoInterop
    {
        [NativeSymbol(Entrypoint = "type_info_create")]
        IntPtr Create([MarshalAs(UnmanagedType.LPStr)]string fullTypeName);
        [NativeSymbol(Entrypoint = "type_info_destroy")]
        void Destroy(IntPtr netTypeInfo);
        
        [NativeSymbol(Entrypoint = "type_info_getFullTypeName")]
        [return: MarshalAs(UnmanagedType.LPStr)]string GetFullTypeName(IntPtr netTypeInfo);
        
        [NativeSymbol(Entrypoint = "type_info_setClassName")]
        void SetClassName(IntPtr netTypeInfo, [MarshalAs(UnmanagedType.LPWStr)]string className);
        [NativeSymbol(Entrypoint = "type_info_getClassName")]
        [return: MarshalAs(UnmanagedType.LPWStr)]string GetClassName(IntPtr netTypeInfo);
        
        [NativeSymbol(Entrypoint = "type_info_setPrefVariantType")]
        void SetPrefVariantType(IntPtr netTypeInfo, NetVariantType variantType);
        [NativeSymbol(Entrypoint = "type_info_getPrefVariantType")]
        NetVariantType GetPrefVariantType(IntPtr netTypeInfo);

        [NativeSymbol(Entrypoint = "type_info_addMethod")]
        void AddMethod(IntPtr typeInfo, IntPtr methodInfo);
        [NativeSymbol(Entrypoint = "type_info_getMethodCount")]
        uint GetMethodCount(IntPtr typeInfo);
        [NativeSymbol(Entrypoint = "type_info_getMethodInfo")]
        IntPtr GetMethodInfo(IntPtr typeInfo, uint index);
        
        [NativeSymbol(Entrypoint = "type_info_addProperty")]
        void AddProperty(IntPtr typeInfo, IntPtr property);
        [NativeSymbol(Entrypoint = "type_info_getPropertyCount")]
        uint GetPropertyCount(IntPtr typeInfo);
        [NativeSymbol(Entrypoint = "type_info_getProperty")]
        IntPtr GetProperty(IntPtr typeInfo, uint index);
    }
}
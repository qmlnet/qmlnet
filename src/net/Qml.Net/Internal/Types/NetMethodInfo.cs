using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Types
{
    internal class NetMethodInfo : BaseDisposable
    {
        public NetMethodInfo(
            NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo,
            bool isStatic)
            : this(Create(parentTypeInfo, methodName, returnTypeInfo, isStatic))
        {
        }

        public NetMethodInfo(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
        }

        private static IntPtr Create(
            NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo,
            bool isStatic)
        {
            return Interop.NetMethodInfo.Create(
                parentTypeInfo?.Handle ?? IntPtr.Zero,
                methodName,
                returnTypeInfo?.Handle ?? IntPtr.Zero,
                isStatic ? (byte)1 : (byte)0);
        }

        public int Id => Interop.NetMethodInfo.GetId(Handle);

        public NetTypeInfo ParentType => new NetTypeInfo(Interop.NetMethodInfo.GetParentType(Handle));

        public string MethodName => Utilities.ContainerToString(Interop.NetMethodInfo.GetMethodName(Handle));

        public NetTypeInfo ReturnType
        {
            get
            {
                var result = Interop.NetMethodInfo.GetReturnType(Handle);
                if (result == IntPtr.Zero) return null;
                return new NetTypeInfo(result);
            }
        }

        public bool IsStatic => Interop.NetMethodInfo.GetIsStatic(Handle) == 1;

        public void AddParameter(string name, NetTypeInfo type)
        {
            Interop.NetMethodInfo.AddParameter(Handle, name, type.Handle);
        }

        public int ParameterCount => Interop.NetMethodInfo.GetParameterCount(Handle);

        public NetMethodInfoParameter GetParameter(int index)
        {
            var result = Interop.NetMethodInfo.GetParameter(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfoParameter(result);
        }

        public List<NetMethodInfoParameter> GetAllParameters()
        {
            var result = new List<NetMethodInfoParameter>();
            var count = ParameterCount;
            for (var x = 0; x < count; x++)
            {
                result.Add(GetParameter(x));
            }
            return result;
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetMethodInfo.Destroy(ptr);
        }
    }

    internal class NetMethodInfoParameter : BaseDisposable
    {
        public NetMethodInfoParameter(IntPtr handle)
            : base(handle)
        {
        }

        public string Name => Utilities.ContainerToString(Interop.NetMethodInfo.GetParameterName(Handle));

        public NetTypeInfo Type
        {
            get
            {
                var result = Interop.NetMethodInfo.GetParameterType(Handle);
                if (result == IntPtr.Zero) return null;
                return new NetTypeInfo(result);
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetMethodInfo.DestroyParameter(ptr);
        }
    }

    internal class NetMethodInfoInterop
    {
        [NativeSymbol(Entrypoint = "method_info_parameter_destroy")]
        public DestroyParameterDel DestroyParameter { get; set; }

        public delegate void DestroyParameterDel(IntPtr parameter);

        [NativeSymbol(Entrypoint = "method_info_parameter_getName")]
        public GetParameterNameDel GetParameterName { get; set; }

        public delegate IntPtr GetParameterNameDel(IntPtr methodParameter);

        [NativeSymbol(Entrypoint = "method_info_parameter_getType")]
        public GetParameterTypeDel GetParameterType { get; set; }

        public delegate IntPtr GetParameterTypeDel(IntPtr methodParameter);

        [NativeSymbol(Entrypoint = "method_info_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel(IntPtr parentTypeInfo, [MarshalAs(UnmanagedType.LPWStr)]string methodName, IntPtr returnTypeInfo, byte isStatic);

        [NativeSymbol(Entrypoint = "method_info_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate int GetIdDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_getId")]
        public GetIdDel GetId { get; set; }

        public delegate IntPtr GetParentTypeDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_getParentType")]
        public GetParentTypeDel GetParentType { get; set; }

        public delegate void DestroyDel(IntPtr methodInfo);

        [NativeSymbol(Entrypoint = "method_info_getMethodName")]
        public GetMethodNameDel GetMethodName { get; set; }

        public delegate IntPtr GetMethodNameDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_getReturnType")]
        public GetReturnTypeDel GetReturnType { get; set; }

        public delegate IntPtr GetReturnTypeDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_isStatic")]
        public GetIsStaticDel GetIsStatic { get; set; }

        public delegate byte GetIsStaticDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_addParameter")]
        public AddParameterDel AddParameter { get; set; }

        public delegate void AddParameterDel(IntPtr method, [MarshalAs(UnmanagedType.LPWStr)]string name, IntPtr type);

        [NativeSymbol(Entrypoint = "method_info_getParameterCount")]
        public GetParameterCountDel GetParameterCount { get; set; }

        public delegate int GetParameterCountDel(IntPtr method);

        [NativeSymbol(Entrypoint = "method_info_getParameter")]
        public GetParameterDel GetParameter { get; set; }

        public delegate IntPtr GetParameterDel(IntPtr method, int index);
    }
}
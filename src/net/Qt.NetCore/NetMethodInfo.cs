using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore
{
    public class NetMethodInfo : BaseDisposable
    {
        public NetMethodInfo(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo)
            : this(Create(parentTypeInfo, methodName, returnTypeInfo))
        {
            
        }

        public NetMethodInfo(IntPtr handle, bool ownsHandle = true)
            :base(handle, ownsHandle)
        {
            
        }

        private static IntPtr Create(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo)
        {
            return Interop.NetMethodInfo.Create(parentTypeInfo?.Handle ?? IntPtr.Zero,
                methodName,
                returnTypeInfo?.Handle ?? IntPtr.Zero);
        }

        public string MethodName => Interop.NetMethodInfo.GetMethodName(Handle);

        public NetTypeInfo ReturnType
        {
            get
            {
                var result = Interop.NetMethodInfo.GetReturnType(Handle);
                if (result == IntPtr.Zero) return null;
                return new NetTypeInfo(result);
            }
        }

        public void AddParameter(string name, NetTypeInfo type)
        {
            Interop.NetMethodInfo.AddParameter(Handle, name, type.Handle);
        }

        public uint ParameterCount => Interop.NetMethodInfo.GetParameterCount(Handle);

        public NetMethodInfoParameter GetParameter(uint index)
        {
            var result = Interop.NetMethodInfo.GetParameter(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfoParameter(result);
        }
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetMethodInfo.Destroy(ptr);
        }
    }

    public class NetMethodInfoParameter : BaseDisposable
    {
        public NetMethodInfoParameter(IntPtr handle)
            : base(handle)
        {
            
        }

        public string Name => Interop.NetMethodInfo.GetParameterName(Handle);

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
    
    public interface INetMethodInfoInterop
    {
        [NativeSymbol(Entrypoint = "method_info_parameter_destroy")]
        void DestroyParameter(IntPtr parameter);
        [NativeSymbol(Entrypoint = "method_info_parameter_getName")]
        [return: MarshalAs(UnmanagedType.LPWStr)]string GetParameterName(IntPtr methodParameter);
        [NativeSymbol(Entrypoint = "method_info_parameter_getType")]
        IntPtr GetParameterType(IntPtr methodParameter);
        
        [NativeSymbol(Entrypoint = "method_info_create")]
        IntPtr Create(IntPtr parentTypeInfo, [MarshalAs(UnmanagedType.LPWStr)]string methodName, IntPtr returnTypeInfo);
        [NativeSymbol(Entrypoint = "method_info_destroy")]
        void Destroy(IntPtr methodInfo);

        [NativeSymbol(Entrypoint = "method_info_getMethodName")]
        [return: MarshalAs(UnmanagedType.LPWStr)]string GetMethodName(IntPtr method);
        [NativeSymbol(Entrypoint = "method_info_getReturnType")]
        IntPtr GetReturnType(IntPtr method);
        
        [NativeSymbol(Entrypoint = "method_info_addParameter")]
        void AddParameter(IntPtr method, [MarshalAs(UnmanagedType.LPWStr)]string name, IntPtr type);
        [NativeSymbol(Entrypoint = "method_info_getParameterCount")]
        uint GetParameterCount(IntPtr method);
        [NativeSymbol(Entrypoint = "method_info_getParameter")]
        IntPtr GetParameter(IntPtr method, uint index);
    }
}
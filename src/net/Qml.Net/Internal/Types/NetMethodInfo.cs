using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qml.Net.Internal;

namespace Qml.Net.Internal.Types
{
    internal class NetMethodInfo : BaseDisposable
    {
        public NetMethodInfo(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo,
            bool isStatic)
            : this(Create(parentTypeInfo, methodName, returnTypeInfo, isStatic))
        {
            
        }

        public NetMethodInfo(IntPtr handle, bool ownsHandle = true)
            :base(handle, ownsHandle)
        {
            
        }

        private static IntPtr Create(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo,
            bool isStatic)
        {
            return Interop.NetMethodInfo.Create(parentTypeInfo?.Handle ?? IntPtr.Zero,
                methodName,
                returnTypeInfo?.Handle ?? IntPtr.Zero,
                isStatic);
        }

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

        public bool IsStatic => Interop.NetMethodInfo.GetIsStatic(Handle);

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
    
    internal interface INetMethodInfoInterop
    {
        [NativeSymbol(Entrypoint = "method_info_parameter_destroy")]
        void DestroyParameter(IntPtr parameter);
        [NativeSymbol(Entrypoint = "method_info_parameter_getName")]
        IntPtr GetParameterName(IntPtr methodParameter);
        [NativeSymbol(Entrypoint = "method_info_parameter_getType")]
        IntPtr GetParameterType(IntPtr methodParameter);
        
        [NativeSymbol(Entrypoint = "method_info_create")]
        IntPtr Create(IntPtr parentTypeInfo, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string methodName, IntPtr returnTypeInfo, bool isStatic);
        [NativeSymbol(Entrypoint = "method_info_destroy")]
        void Destroy(IntPtr methodInfo);

        [NativeSymbol(Entrypoint = "method_info_getMethodName")]
        IntPtr GetMethodName(IntPtr method);
        [NativeSymbol(Entrypoint = "method_info_getReturnType")]
        IntPtr GetReturnType(IntPtr method);
        [NativeSymbol(Entrypoint = "method_info_isStatic")]
        bool GetIsStatic(IntPtr method);
        
        [NativeSymbol(Entrypoint = "method_info_addParameter")]
        void AddParameter(IntPtr method, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string name, IntPtr type);
        [NativeSymbol(Entrypoint = "method_info_getParameterCount")]
        int GetParameterCount(IntPtr method);
        [NativeSymbol(Entrypoint = "method_info_getParameter")]
        IntPtr GetParameter(IntPtr method, int index);
    }
}
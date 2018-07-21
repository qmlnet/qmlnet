using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public IntPtr IsTypeValid;
        public IntPtr BuildTypeInfo;
        public IntPtr ReleaseGCHandle;
        public IntPtr InstantiateType;
        public IntPtr ReadProperty;
        public IntPtr WriteProperty;
        public IntPtr InvokeMethod;
        public IntPtr GCCollect;
    }

    public interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPWStr)]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseGCHandle")]
        void ReleaseGCHandle(IntPtr handle);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_buildTypeInfo")]
        void BuildTypeInfo(IntPtr handle);

        [NativeSymbol(Entrypoint = "type_info_callbacks_instantiateType")]
        IntPtr InstantiateType(IntPtr type);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_readProperty")]
        void ReadProperty(IntPtr property, IntPtr target, IntPtr result);

        [NativeSymbol(Entrypoint = "type_info_callbacks_writeProperty")]
        void WriteProperty(IntPtr property, IntPtr target, IntPtr value);

        [NativeSymbol(Entrypoint = "type_info_callbacks_invokeMethod")]
        void InvokeMethod(IntPtr method, IntPtr target, IntPtr variants, IntPtr result);
    }

    public interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseGCHandle(IntPtr handle);
        
        void BuildTypeInfo(IntPtr typeInfo);

        IntPtr InstantiateType(IntPtr type);

        void ReadProperty(IntPtr property, IntPtr target, IntPtr result);

        void WriteProperty(IntPtr property, IntPtr target, IntPtr value);

        void InvokeMethod(IntPtr method, IntPtr target, IntPtr parameters, IntPtr result);

        void GCCollect(int maxGeneration);
    }
    
    public class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        BuildTypeInfoDelegate _buildTypeInfoDelegate;
        ReleaseGCHandleDelegate _releaseGCHandleDelegate;
        InstantiateTypeDelgate _instantiateTypeDelgate;
        ReadPropertyDelegate _readPropertyDelegate;
        WritePropertyDelegate _writePropertyDelegate;
        InvokeMethodDelegate _invokeMethodDelegate;
        GCCollectDelegate _gcCollectDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPWStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void BuildTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseGCHandleDelegate(IntPtr handle);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr InstantiateTypeDelgate(IntPtr type);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReadPropertyDelegate(IntPtr property, IntPtr target, IntPtr result);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WritePropertyDelegate(IntPtr property, IntPtr target, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void InvokeMethodDelegate(IntPtr method, IntPtr target, IntPtr variants, IntPtr result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GCCollectDelegate(int maxGeneration);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);

            _releaseGCHandleDelegate = ReleaseGCHandle;
            GCHandle.Alloc(_releaseGCHandleDelegate);
            
            _buildTypeInfoDelegate = BuildTypeInfo;
            GCHandle.Alloc(_buildTypeInfoDelegate);
            
            _instantiateTypeDelgate = InstantiateType;
            GCHandle.Alloc(_instantiateTypeDelgate);

            _readPropertyDelegate = ReadProperty;
            GCHandle.Alloc(_readPropertyDelegate);

            _writePropertyDelegate = WriteProperty;
            GCHandle.Alloc(_writePropertyDelegate);

            _invokeMethodDelegate = InvokeMethod;
            GCHandle.Alloc(_invokeMethodDelegate);

            _gcCollectDelegate = GCCollect;
            GCHandle.Alloc(_gcCollectDelegate);
        }

        private bool IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName);
        }
        
        private void ReleaseGCHandle(IntPtr handle)
        {
            _callbacks.ReleaseGCHandle(handle);
        }

        private void BuildTypeInfo(IntPtr type)
        {
            _callbacks.BuildTypeInfo(type);
        }
        
        private IntPtr InstantiateType(IntPtr type)
        {
            return _callbacks.InstantiateType(type);
        }
        
        private void ReadProperty(IntPtr property, IntPtr target, IntPtr result)
        {
            _callbacks.ReadProperty(property, target, result);
        }

        private void WriteProperty(IntPtr property, IntPtr target, IntPtr value)
        {
            _callbacks.WriteProperty(property, target, value);
        }

        private void InvokeMethod(IntPtr method, IntPtr target, IntPtr variants, IntPtr result)
        {
            _callbacks.InvokeMethod(method, target, variants, result);
        }

        private void GCCollect(int maxGeneration)
        {
            _callbacks.GCCollect(maxGeneration);
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                BuildTypeInfo = Marshal.GetFunctionPointerForDelegate(_buildTypeInfoDelegate),
                ReleaseGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseGCHandleDelegate),
                InstantiateType = Marshal.GetFunctionPointerForDelegate(_instantiateTypeDelgate),
                ReadProperty = Marshal.GetFunctionPointerForDelegate(_readPropertyDelegate),
                WriteProperty = Marshal.GetFunctionPointerForDelegate(_writePropertyDelegate),
                InvokeMethod = Marshal.GetFunctionPointerForDelegate(_invokeMethodDelegate),
                GCCollect = Marshal.GetFunctionPointerForDelegate(_gcCollectDelegate)
            };
        }
    }
}
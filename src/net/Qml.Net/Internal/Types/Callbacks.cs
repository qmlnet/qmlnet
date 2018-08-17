using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Callbacks
    {
        public IntPtr IsTypeValid;
        public IntPtr CreateLazyTypeInfo;
        public IntPtr LoadTypeInfo;
        public IntPtr ReleaseNetReference;
        public IntPtr ReleaseNetDelegateGCHandle;
        public IntPtr InstantiateType;
        public IntPtr ReadProperty;
        public IntPtr WriteProperty;
        public IntPtr InvokeMethod;
        public IntPtr GCCollect;
        public IntPtr RaseNetSignals;
        public IntPtr AwaitTask;
        public IntPtr Serialize;
    }

    internal interface ICallbacksIterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        void RegisterCallbacks(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        bool IsTypeValid([MarshalAs(UnmanagedType.LPWStr), CallerFree]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseNetReferenceGCHandle")]
        void ReleaseNetReference(UInt64 objectId);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseNetDelegateGCHandle")]
        void ReleaseNetDelegateGCHandle(IntPtr handle);

        [NativeSymbol(Entrypoint = "type_info_callbacks_instantiateType")]
        IntPtr InstantiateType(IntPtr type);
        
        [NativeSymbol(Entrypoint = "type_info_callbacks_readProperty")]
        void ReadProperty(IntPtr property, IntPtr target, IntPtr result);

        [NativeSymbol(Entrypoint = "type_info_callbacks_writeProperty")]
        void WriteProperty(IntPtr property, IntPtr target, IntPtr value);

        [NativeSymbol(Entrypoint = "type_info_callbacks_invokeMethod")]
        void InvokeMethod(IntPtr method, IntPtr target, IntPtr variants, IntPtr result);
    }

    internal interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseNetReference(UInt64 objectId);

        void ReleaseNetDelegateGCHandle(IntPtr handle);

        void CreateLazyTypeInfo(IntPtr typeInfo);

        void LoadTypeInfo(IntPtr typeInfo);
        
        IntPtr InstantiateType(IntPtr type);

        void ReadProperty(IntPtr property, IntPtr target, IntPtr result);

        void WriteProperty(IntPtr property, IntPtr target, IntPtr value);

        void InvokeMethod(IntPtr method, IntPtr target, IntPtr parameters, IntPtr result);

        void GCCollect(int maxGeneration);

        bool RaiseNetSignals(IntPtr target, string signalName, IntPtr parameters);

        Task AwaitTask(IntPtr target, IntPtr succesCallback, IntPtr failureCallback);
        
        bool Serialize(IntPtr instance, IntPtr result);
    }
    
    internal class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        CreateLazyTypeInfoDelegate _createLazyTypeInfoDelegate;
        LoadTypeInfoDelegate _loadTypeInfoDelegate;
        ReleaseNetReferenceDelegate _releaseNetReferenceDelegate;
        ReleaseNetDelegateGCHandleDelegate _releaseNetDelegateGCHandleDelegate;
        InstantiateTypeDelgate _instantiateTypeDelgate;
        ReadPropertyDelegate _readPropertyDelegate;
        WritePropertyDelegate _writePropertyDelegate;
        InvokeMethodDelegate _invokeMethodDelegate;
        GCCollectDelegate _gcCollectDelegate;
        RaiseNetSignalsDelegate _raiseNetSignalsDelegate;
        AwaitTaskDelegate _awaitTaskDelegate;
        SerializeDelegate _serializeDelegate;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IsTypeValidDelegate([MarshalAs(UnmanagedType.LPWStr)]string typeName);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void CreateLazyTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LoadTypeInfoDelegate(IntPtr typeInfo);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseNetReferenceDelegate(UInt64 objectId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ReleaseNetDelegateGCHandleDelegate(IntPtr handle);

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
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool RaiseNetSignalsDelegate(IntPtr target, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr parameters);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void AwaitTaskDelegate(IntPtr target, IntPtr successCallback, IntPtr failureCallback);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool SerializeDelegate(IntPtr instance, IntPtr result);
        
        public CallbacksImpl(ICallbacks callbacks)
        {
            _callbacks = callbacks;
            
            _isTypeValidDelegate = IsTypeValid;
            GCHandle.Alloc(_isTypeValidDelegate);

            _releaseNetReferenceDelegate = ReleaseNetReference;
            GCHandle.Alloc(_releaseNetReferenceDelegate);

            _releaseNetDelegateGCHandleDelegate = ReleaseNetDelegateGCHandle;
            GCHandle.Alloc(_releaseNetDelegateGCHandleDelegate);

            _createLazyTypeInfoDelegate = CreateLazyTypeInfo;
            GCHandle.Alloc(_createLazyTypeInfoDelegate);

            _loadTypeInfoDelegate = LoadTypeInfo;
            GCHandle.Alloc(_loadTypeInfoDelegate);
            
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

            _raiseNetSignalsDelegate = RaiseNetSignals;
            GCHandle.Alloc(_raiseNetSignalsDelegate);

            _awaitTaskDelegate = AwaitTask;
            GCHandle.Alloc(_awaitTaskDelegate);

            _serializeDelegate = Serialize;
            GCHandle.Alloc(_serializeDelegate);
        }

        private bool IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName);
        }
        
        private void ReleaseNetReference(UInt64 objectId)
        {
            _callbacks.ReleaseNetReference(objectId);
        }

        private void ReleaseNetDelegateGCHandle(IntPtr handle)
        {
            _callbacks.ReleaseNetDelegateGCHandle(handle);
        }

        private void CreateLazyTypeInfo(IntPtr type)
        {
            _callbacks.CreateLazyTypeInfo(type);
        }
        
        private void LoadTypeInfo(IntPtr type)
        {
            _callbacks.LoadTypeInfo(type);
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

        private bool RaiseNetSignals(IntPtr target,
            string signalName,
            IntPtr parameters)
        {
            return _callbacks.RaiseNetSignals(target, signalName, parameters);
        }

        private void AwaitTask(IntPtr target, IntPtr succesCallback, IntPtr failureCallback)
        {
            _callbacks.AwaitTask(target, succesCallback, failureCallback);
        }

        private bool Serialize(IntPtr instance, IntPtr result)
        {
            return _callbacks.Serialize(instance, result);
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                CreateLazyTypeInfo = Marshal.GetFunctionPointerForDelegate(_createLazyTypeInfoDelegate),
                LoadTypeInfo = Marshal.GetFunctionPointerForDelegate(_loadTypeInfoDelegate),
                ReleaseNetReference = Marshal.GetFunctionPointerForDelegate(_releaseNetReferenceDelegate),
                ReleaseNetDelegateGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseNetDelegateGCHandleDelegate),
                InstantiateType = Marshal.GetFunctionPointerForDelegate(_instantiateTypeDelgate),
                ReadProperty = Marshal.GetFunctionPointerForDelegate(_readPropertyDelegate),
                WriteProperty = Marshal.GetFunctionPointerForDelegate(_writePropertyDelegate),
                InvokeMethod = Marshal.GetFunctionPointerForDelegate(_invokeMethodDelegate),
                GCCollect = Marshal.GetFunctionPointerForDelegate(_gcCollectDelegate),
                RaseNetSignals = Marshal.GetFunctionPointerForDelegate(_raiseNetSignalsDelegate),
                AwaitTask = Marshal.GetFunctionPointerForDelegate(_awaitTaskDelegate),
                Serialize = Marshal.GetFunctionPointerForDelegate(_serializeDelegate)
            };
        }
    }
}
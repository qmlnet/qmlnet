using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace Qml.Net.Internal.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Callbacks
    {
        public IntPtr IsTypeValid;
        public IntPtr CreateLazyTypeInfo;
        public IntPtr LoadTypeInfo;
        public IntPtr CallComponentCompleted;
        public IntPtr CallObjectDestroyed;
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
        public IntPtr InvokeDelegate;
    }

    internal class CallbacksInterop
    {
        [NativeSymbol(Entrypoint = "type_info_callbacks_registerCallbacks")]
        public RegisterCallbacksDel RegisterCallbacks { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegisterCallbacksDel(ref Callbacks callbacks);

        [NativeSymbol(Entrypoint = "type_info_callbacks_isTypeValid")]
        public IsTypeValidDel IsTypeValid { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool IsTypeValidDel([MarshalAs(UnmanagedType.LPWStr)]string typeName);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseNetReferenceGCHandle")]
        public ReleaseNetReferenceDel ReleaseNetReference { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReleaseNetReferenceDel(UInt64 objectId);

        [NativeSymbol(Entrypoint = "type_info_callbacks_releaseNetDelegateGCHandle")]
        public ReleaseNetDelegateGCHandleDel ReleaseNetDelegateGCHandle { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReleaseNetDelegateGCHandleDel(IntPtr handle);

        [NativeSymbol(Entrypoint = "type_info_callbacks_instantiateType")]
        public InstantiateTypeDel InstantiateType { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr InstantiateTypeDel(IntPtr type, int aotTypeId);

        [NativeSymbol(Entrypoint = "type_info_callbacks_invokeMethod")]
        public InvokeMethodDel InvokeMethod { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InvokeMethodDel(IntPtr method, IntPtr target, IntPtr variants, IntPtr result);
    }

    internal interface ICallbacks
    {
        bool IsTypeValid(string typeName);

        void ReleaseNetReference(UInt64 objectId);

        void ReleaseNetDelegateGCHandle(IntPtr handle);

        void CreateLazyTypeInfo(IntPtr typeInfo);

        void LoadTypeInfo(IntPtr typeInfo);

        IntPtr InstantiateType(IntPtr type, int aotTypeId);

        void CallComponentCompleted(IntPtr target);

        void CallObjectDestroyed(IntPtr target);

        void ReadProperty(IntPtr property, IntPtr target, IntPtr indexProperty, IntPtr result);

        void WriteProperty(IntPtr property, IntPtr target, IntPtr indexProperty, IntPtr value);

        void InvokeMethod(IntPtr method, IntPtr target, IntPtr parameters, IntPtr result);

        void GCCollect(int maxGeneration);

        bool RaiseNetSignals(IntPtr target, string signalName, IntPtr parameters);

        Task AwaitTask(IntPtr target, IntPtr succesCallback, IntPtr failureCallback);

        bool Serialize(IntPtr instance, IntPtr result);

        void InvokeDelegate(IntPtr del, IntPtr parameters);
    }

    internal class CallbacksImpl
    {
        readonly ICallbacks _callbacks;
        IsTypeValidDelegate _isTypeValidDelegate;
        CreateLazyTypeInfoDelegate _createLazyTypeInfoDelegate;
        LoadTypeInfoDelegate _loadTypeInfoDelegate;
        CallComponentCompletedDelegate _callComponentCompletedDelegate;
        CallObjectDestroyedDelegate _callObjectDestroyedDelegate;
        ReleaseNetReferenceDelegate _releaseNetReferenceDelegate;
        ReleaseNetDelegateGCHandleDelegate _releaseNetDelegateGCHandleDelegate;
        InstantiateTypeDelegate _instantiateTypeDelegate;
        ReadPropertyDelegate _readPropertyDelegate;
        WritePropertyDelegate _writePropertyDelegate;
        InvokeMethodDelegate _invokeMethodDelegate;
        GCCollectDelegate _gcCollectDelegate;
        RaiseNetSignalsDelegate _raiseNetSignalsDelegate;
        AwaitTaskDelegate _awaitTaskDelegate;
        SerializeDelegate _serializeDelegate;
        InvokeDelegateDelegate _invokeDelegateDelegate;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate byte IsTypeValidDelegate([MarshalAs(UnmanagedType.LPWStr)]string typeName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void CreateLazyTypeInfoDelegate(IntPtr typeInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void LoadTypeInfoDelegate(IntPtr typeInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void CallComponentCompletedDelegate(IntPtr target);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void CallObjectDestroyedDelegate(IntPtr target);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void ReleaseNetReferenceDelegate(UInt64 objectId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void ReleaseNetDelegateGCHandleDelegate(IntPtr handle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate IntPtr InstantiateTypeDelegate(IntPtr type, int aotTypeId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void ReadPropertyDelegate(IntPtr property, IntPtr target, IntPtr indexParameter, IntPtr result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void WritePropertyDelegate(IntPtr property, IntPtr target, IntPtr indexParameter, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void InvokeMethodDelegate(IntPtr method, IntPtr target, IntPtr variants, IntPtr result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void GCCollectDelegate(int maxGeneration);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate byte RaiseNetSignalsDelegate(IntPtr target, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr parameters);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void AwaitTaskDelegate(IntPtr target, IntPtr successCallback, IntPtr failureCallback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate byte SerializeDelegate(IntPtr instance, IntPtr result);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        delegate void InvokeDelegateDelegate(IntPtr del, IntPtr parameters);

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

            _instantiateTypeDelegate = InstantiateType;
            GCHandle.Alloc(_instantiateTypeDelegate);

            _callComponentCompletedDelegate = CallComponentCompleted;
            GCHandle.Alloc(_callComponentCompletedDelegate);

            _callObjectDestroyedDelegate = CallObjectDestroyed;
            GCHandle.Alloc(_callObjectDestroyedDelegate);

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

            _invokeDelegateDelegate = InvokeDelegate;
            GCHandle.Alloc(_invokeDelegateDelegate);
        }

        private byte IsTypeValid(string typeName)
        {
            return _callbacks.IsTypeValid(typeName) ? (byte)1 : (byte)0;
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

        private IntPtr InstantiateType(IntPtr type, int aotTypeId)
        {
            return _callbacks.InstantiateType(type, aotTypeId);
        }

        private void CallComponentCompleted(IntPtr target)
        {
            _callbacks.CallComponentCompleted(target);
        }

        private void CallObjectDestroyed(IntPtr target)
        {
            _callbacks.CallObjectDestroyed(target);
        }

        private void ReadProperty(IntPtr property, IntPtr target, IntPtr indexParameter, IntPtr result)
        {
            _callbacks.ReadProperty(property, target, indexParameter, result);
        }

        private void WriteProperty(IntPtr property, IntPtr target, IntPtr indexParameter, IntPtr value)
        {
            _callbacks.WriteProperty(property, target, indexParameter, value);
        }

        private void InvokeMethod(IntPtr method, IntPtr target, IntPtr variants, IntPtr result)
        {
            _callbacks.InvokeMethod(method, target, variants, result);
        }

        private void GCCollect(int maxGeneration)
        {
            _callbacks.GCCollect(maxGeneration);
        }

        private byte RaiseNetSignals(
            IntPtr target,
            string signalName,
            IntPtr parameters)
        {
            return _callbacks.RaiseNetSignals(target, signalName, parameters) ? (byte)1 : (byte)0;
        }

        private void AwaitTask(IntPtr target, IntPtr succesCallback, IntPtr failureCallback)
        {
            _callbacks.AwaitTask(target, succesCallback, failureCallback);
        }

        private byte Serialize(IntPtr instance, IntPtr result)
        {
            return _callbacks.Serialize(instance, result) ? (byte)1 : (byte)0;
        }

        private void InvokeDelegate(IntPtr del, IntPtr parameters)
        {
            _callbacks.InvokeDelegate(del, parameters);
        }

        public Callbacks Callbacks()
        {
            return new Callbacks
            {
                IsTypeValid = Marshal.GetFunctionPointerForDelegate(_isTypeValidDelegate),
                CreateLazyTypeInfo = Marshal.GetFunctionPointerForDelegate(_createLazyTypeInfoDelegate),
                LoadTypeInfo = Marshal.GetFunctionPointerForDelegate(_loadTypeInfoDelegate),
                CallComponentCompleted = Marshal.GetFunctionPointerForDelegate(_callComponentCompletedDelegate),
                CallObjectDestroyed = Marshal.GetFunctionPointerForDelegate(_callObjectDestroyedDelegate),
                ReleaseNetReference = Marshal.GetFunctionPointerForDelegate(_releaseNetReferenceDelegate),
                ReleaseNetDelegateGCHandle = Marshal.GetFunctionPointerForDelegate(_releaseNetDelegateGCHandleDelegate),
                InstantiateType = Marshal.GetFunctionPointerForDelegate(_instantiateTypeDelegate),
                ReadProperty = Marshal.GetFunctionPointerForDelegate(_readPropertyDelegate),
                WriteProperty = Marshal.GetFunctionPointerForDelegate(_writePropertyDelegate),
                InvokeMethod = Marshal.GetFunctionPointerForDelegate(_invokeMethodDelegate),
                GCCollect = Marshal.GetFunctionPointerForDelegate(_gcCollectDelegate),
                RaseNetSignals = Marshal.GetFunctionPointerForDelegate(_raiseNetSignalsDelegate),
                AwaitTask = Marshal.GetFunctionPointerForDelegate(_awaitTaskDelegate),
                Serialize = Marshal.GetFunctionPointerForDelegate(_serializeDelegate),
                InvokeDelegate = Marshal.GetFunctionPointerForDelegate(_invokeDelegateDelegate)
            };
        }
    }
}
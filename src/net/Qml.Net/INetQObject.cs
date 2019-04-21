using System;
using System.Collections.Generic;

namespace Qml.Net
{
    public interface INetQObject : IDisposable
    {
        object GetProperty(string propertyName);

        void SetProperty(string propertyName, object value);

        object InvokeMethod(string methodName, params object[] parameters);
        
        IDisposable AttachSignal(string signalName, SignalHandler handler);

        IDisposable AttachNotifySignal(string propertyName, SignalHandler handler);
    }
    
    public delegate void SignalHandler(List<object> parameters);
}
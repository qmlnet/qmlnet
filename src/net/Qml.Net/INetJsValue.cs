using System;

namespace Qml.Net
{
    public interface INetJsValue : IDisposable
    {
        bool IsCallable { get; }

        bool IsArray { get; }

        object GetProperty(string propertyName);

        void SetProperty(string propertyName, object value);

        object GetItemAtIndex(int arrayIndex);

        object Call(params object[] parameters);
    }
}
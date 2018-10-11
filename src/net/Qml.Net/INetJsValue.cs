using System;

namespace Qml.Net
{
    public interface INetJsValue : IDisposable
    {
        bool IsCallable { get; }

        bool IsArray { get; }
        
        bool IsDouble { get; }
        
        bool IsString { get; }

        object GetProperty(string propertyName);

        object GetItemAtIndex(int arrayIndex);

        double ToDouble();

        string ToString();
        
        object Call(params object[] parameters);
        
    }
}
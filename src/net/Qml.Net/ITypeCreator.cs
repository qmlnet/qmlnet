using System;

namespace Qml.Net
{
    public interface ITypeCreator
    {
        object Create(Type type);
    }
}
using System;

namespace Qt.NetCore
{
    public interface ITypeCreator
    {
        object Create(Type type);
    }
}
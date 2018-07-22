using System;

namespace Qml.Net
{
    public interface ITypeCreator
    {
        object Create(Type type);
    }

    public static class TypeCreator
    {
        public static ITypeCreator Current { get; set; }

        public static object Create(Type type)
        {
            var typeCreator = Current;
            if (typeCreator != null)
            {
                return typeCreator.Create(type);
            }
            return Activator.CreateInstance(type);
        }
    }
}
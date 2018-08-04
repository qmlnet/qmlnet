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

        public static ITypeCreator FromDelegate(Func<Type, object> func)
        {
            return new DelegateTypeCreator(func);
        }

        class DelegateTypeCreator : ITypeCreator
        {
            private readonly Func<Type, object> _func;

            public DelegateTypeCreator(Func<Type, object> func)
            {
                _func = func;
            }
            
            public object Create(Type type)
            {
                return _func(type);
            }
        }
    }
}
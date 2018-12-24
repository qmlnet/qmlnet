using System;
using System.Collections.Generic;

namespace Qml.Net.Tests
{
    public class MockTypeCreator : ITypeCreator
    {
        private Dictionary<Type, object> _instances;

        public MockTypeCreator(params Tuple<Type, object>[] instances)
        {
            _instances = new Dictionary<Type, object>();
            foreach (var tuple in instances)
            {
                _instances[tuple.Item1] = tuple.Item2;
            }
        }

        public void SetInstance(Type t, object instance)
        {
            _instances[t] = instance;
        }

        public object Create(Type type)
        {
            if (_instances.ContainsKey(type))
            {
                return _instances[type];
            }
            throw new ArgumentException("Unknown Type: " + type.AssemblyQualifiedName, "type");
        }
    }
}
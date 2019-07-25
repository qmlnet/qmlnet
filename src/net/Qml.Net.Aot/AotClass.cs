using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Qml.Net.Aot
{
    public class AotClass
    {
        private readonly Type _type;
        private readonly List<AotMethod> _methods = new List<AotMethod>();

        public AotClass(Type type)
        {
            _type = type;
        }

        public Type Type => _type;
        
        public ReadOnlyCollection<AotMethod> Methods => new ReadOnlyCollection<AotMethod>(_methods);

        internal void AddMethod(MethodInfo methodInfo)
        {
            _methods.Add(new AotMethod(this, methodInfo));
        }
    }
}
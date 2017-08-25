using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore.Tests
{
    public class MockTypeCreator : ITypeCreator
    {
        private readonly object _instance;

        public MockTypeCreator(object instance)
        {
            _instance = instance;
        }

        public object Create(Type type)
        {
            return _instance;
        }
    }
}

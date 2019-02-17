using System;
using System.Linq.Expressions;
using Moq;
using Qml.Net.Internal.Types;

namespace Qml.Net.Tests.CodeGen
{
    public class CodeGenBase<T>
        where T : class
    {
        protected Mock<T> _mock;
        private NetTypeInfo _typeInfo;

        protected CodeGenBase()
        {
            _mock = new Mock<T>();
            _typeInfo = NetTypeManager.GetTypeInfo<T>();
            _typeInfo.EnsureLoaded();
        }

        protected object BuildInvokeMethodDelegate(string methodName)
        {
            return global::Qml.Net.Internal.CodeGen.CodeGen.BuildInvokeMethodDelegate(GetMethod((methodName)));
        }

        protected object BuildReadPropertyDelegate(string propertyName)
        {
            return global::Qml.Net.Internal.CodeGen.CodeGen.BuildReadPropertyDelegate(GetProperty((propertyName)));
        }

        protected object BuildSetPropertyDelegate(string propertyName)
        {
            return global::Qml.Net.Internal.CodeGen.CodeGen.BuildSetPropertyDelegate(GetProperty((propertyName)));
        }

        private NetMethodInfo GetMethod(string methodName)
        {
            for (var x = 0; x < _typeInfo.LocalMethodCount; x++)
            {
                var method = _typeInfo.GetMethod(x);
                if (method.MethodName == methodName)
                {
                    return method;
                }

                method.Dispose();
            }

            throw new Exception($"No method {methodName} found.");
        }

        private NetPropertyInfo GetProperty(string propertyName)
        {
            for (var x = 0; x < _typeInfo.PropertyCount; x++)
            {
                var prop = _typeInfo.GetProperty(x);
                if (prop.Name == propertyName)
                {
                    return prop;
                }

                prop.Dispose();
            }

            throw new Exception($"No prop {propertyName} found.");
        }
    }
}
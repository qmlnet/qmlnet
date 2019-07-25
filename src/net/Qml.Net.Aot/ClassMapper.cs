using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Qml.Net.Aot
{
    public class ClassMapper<T>
    {
        private readonly AotClass _aotClass;

        public ClassMapper(AotClass aotClass)
        {
            _aotClass = aotClass;
        }

        public ClassMapper<T> MapMethod<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var methodCallExpression = (MethodCallExpression)expression.Body;
            _aotClass.AddMethod(methodCallExpression.Method);
            return this;
        }
        
        public ClassMapper<T> MapMethod(Expression<Action<T>> expression)
        {
            var methodCallExpression = (MethodCallExpression)expression.Body;
            _aotClass.AddMethod(methodCallExpression.Method);
            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Moq;

namespace Qt.NetCore.Tests
{
    public abstract class BaseTests : IDisposable
    {
        readonly StringVector _applicationArgs;
        readonly QGuiApplication _coreApplication;
        // ReSharper disable InconsistentNaming
        protected readonly QQmlApplicationEngine qmlApplicationEngine;
        // ReSharper restore InconsistentNaming
        readonly List<Type> _registeredTypes = new List<Type>();
        static readonly object LockObject = new object();

        protected BaseTests()
        {
            Monitor.Enter(LockObject);

#if DEBUG
            Helpers.LoadDebugVariables();
#endif

            _applicationArgs = new StringVector();
            _coreApplication = new QGuiApplication(_applicationArgs);

            qmlApplicationEngine = new QQmlApplicationEngine();
        }
        
        public void RegisterType<T>()
        {
            if (_registeredTypes.Contains(typeof(T))) return;
            _registeredTypes.Add(typeof(T));
            QQmlApplicationEngine.RegisterType<T>("tests", 1, 0);
        }

        public virtual void Dispose()
        {
            qmlApplicationEngine.Dispose();
            _coreApplication.Dispose();
            _applicationArgs.Dispose();

            Monitor.Exit(LockObject);
        }
    }

    public abstract class BaseTests<T> : BaseTests where T:class
    {
        protected readonly Mock<T> Mock;

        protected BaseTests()
        {
            RegisterType<T>();
            Mock = new Mock<T>();
            NetTypeInfoManager.TypeCreator = new MockTypeCreator(Mock.Object);
        }

        public override void Dispose()
        {
            base.Dispose();
            NetTypeInfoManager.TypeCreator = null;
        }
    }
}
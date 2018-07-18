using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;

namespace Qt.NetCore.Tests
{
    public abstract class BaseQmlTests : BaseTests
    {
        readonly QGuiApplication _coreApplication;
        // ReSharper disable InconsistentNaming
        protected readonly QQmlApplicationEngine qmlApplicationEngine;
        // ReSharper restore InconsistentNaming
        readonly List<Type> _registeredTypes = new List<Type>();

        protected BaseQmlTests()
        {
            _coreApplication = new QGuiApplication();
            qmlApplicationEngine = new QQmlApplicationEngine();
        }

        protected void RegisterType<T>()
        {
            if (_registeredTypes.Contains(typeof(T))) return;
            _registeredTypes.Add(typeof(T));
            QQmlApplicationEngine.RegisterType<T>("tests");
        }

        public override void Dispose()
        {
            qmlApplicationEngine.Dispose();
            _coreApplication.Dispose();

            base.Dispose();
        }
    }

    public abstract class BaseQmlTests<T> : BaseQmlTests where T:class
    {
        protected readonly Mock<T> Mock;

        protected BaseQmlTests()
        {
            RegisterType<T>();
            Mock = new Mock<T>();
            NetInstance.TypeCreator = new MockTypeCreator(Mock.Object);
        }

        public override void Dispose()
        {
            NetInstance.TypeCreator = null;
            base.Dispose();
        }
    }

    public abstract class BaseQmlTestsWithInstance<T> : BaseQmlTests where T : class, new()
    {
        protected readonly T Instance;

        protected BaseQmlTestsWithInstance()
        {
            RegisterType<T>();
            Instance = new T();
            NetInstance.TypeCreator = new MockTypeCreator(Instance);
        }

        public override void Dispose()
        {
            NetInstance.TypeCreator = null;
            base.Dispose();
        }
    }
}
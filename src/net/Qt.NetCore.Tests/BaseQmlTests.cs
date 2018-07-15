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

        public BaseQmlTests()
        {
            _coreApplication = new QGuiApplication();
            qmlApplicationEngine = new QQmlApplicationEngine();
        }
        
        public void RegisterType<T>()
        {
            if (_registeredTypes.Contains(typeof(T))) return;
            _registeredTypes.Add(typeof(T));
            QQmlApplicationEngine.RegisterType<T>("tests", 1, 0);
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
        protected Mock<T> Mock;

        protected BaseQmlTests()
        {
            RegisterType<T>();
            Mock = new Mock<T>();
            NetInstance.TypeCreator = new MockTypeCreator(Mock.Object);
        }

        public override void Dispose()
        {
            base.Dispose();
            NetInstance.TypeCreator = null;
        }
    }
}
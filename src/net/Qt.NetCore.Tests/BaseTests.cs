using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests
{
    public abstract class BaseTests : IDisposable
    {
        readonly StringVector _applicationArgs;
        readonly QGuiApplication _coreApplication;
        // ReSharper disable InconsistentNaming
        protected QQmlApplicationEngine qmlApplicationEngine;
        // ReSharper restore InconsistentNaming
        readonly List<Type> _registeredTypes = new List<Type>();
        static readonly object LockObject = new object();

        public BaseTests()
        {
            Monitor.Enter(LockObject);

            // temp
            var path = Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Desktop_Qt_5_9_1_MSVC2017_64bit-Debug\debug";
            Environment.SetEnvironmentVariable("PATH", path);

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
        protected Mock<T> Mock;

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

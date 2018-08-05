using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qml.Net.Internal;
using Qml.Net.Internal.Behaviors;

namespace Qml.Net.Tests.Qml
{
    public class TestContext
    {
        private QGuiApplication _coreApplication { get; set; }

        public TestContext(QGuiApplication application)
        {
            _coreApplication = application;
        }

        public void Quit()
        {
            _coreApplication.Dispatch(() =>
            {
                _coreApplication.Exit();
            });
        }
    }

    public abstract class BaseQmlTests : BaseTests
    {
        private readonly QGuiApplication _coreApplication;
        protected readonly QQmlApplicationEngine qmlApplicationEngine;
        protected MockTypeCreator TypeCreator { get;  private set; }

        readonly List<Type> _registeredTypes = new List<Type>();
        static bool _testContextRegistered = false;

        protected BaseQmlTests()
        {
            _coreApplication = new QGuiApplication(new []{ "-platform", "offscreen" });
            qmlApplicationEngine = new QQmlApplicationEngine();
            TypeCreator = new MockTypeCreator();
            Net.TypeCreator.Current = TypeCreator;
            TypeCreator.SetInstance(typeof(TestContext), new TestContext(_coreApplication));
            if (!_testContextRegistered)
            {
                QQmlApplicationEngine.RegisterType<TestContext>("testContext");
                _testContextRegistered = true;
            }
        }

        protected void RegisterType<T>()
        {
            if (_registeredTypes.Contains(typeof(T))) return;
            _registeredTypes.Add(typeof(T));
            QQmlApplicationEngine.RegisterType<T>("tests");
        }

        protected int ExecApplicationWithTimeout(int timeoutMs)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(timeoutMs);
                if(!ct.IsCancellationRequested)
                {
                    _coreApplication.Exit(-1);
                }
            }, ct);

            var result = _coreApplication.Exec();
            cts.Cancel();
            return result;
        }

        public override void Dispose()
        {
            qmlApplicationEngine.Dispose();
            _coreApplication.Dispose();

            Net.TypeCreator.Current = null;

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
            TypeCreator.SetInstance(typeof(T), Mock.Object);
        }

        public override void Dispose()
        {
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
            TypeCreator.SetInstance(typeof(T), Instance);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public abstract class BaseQmlMvvmTestsWithInstance<T> : BaseQmlTests where T : class, new()
    {
        protected readonly T Instance;

        protected BaseQmlMvvmTestsWithInstance()
        {
            InteropBehaviors.ClearQmlInteropBehaviors();
            InteropBehaviors.RegisterQmlInteropBehavior(new MvvmQmlInteropBehavior());

            RegisterType<T>();
            Instance = new T();
            TypeCreator.SetInstance(typeof(T), Instance);
        }

        public override void Dispose()
        {
            InteropBehaviors.ClearQmlInteropBehaviors();
            base.Dispose();
        }
    }
}
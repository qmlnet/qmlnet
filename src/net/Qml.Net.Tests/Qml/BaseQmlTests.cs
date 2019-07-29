using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qml.Net.Internal;
using Qml.Net.Internal.Behaviors;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Tests.Qml
{
    public class BaseQmlEngineTests : BaseTests
    {
        private readonly QGuiApplication _coreApplication;
        protected readonly QQmlApplicationEngine qmlApplicationEngine;

        public BaseQmlEngineTests()
        {
            TestInterop.TestInterop.Register();

            _coreApplication = new QGuiApplication(new[] { "-platform", "offscreen" });
            qmlApplicationEngine = new QQmlApplicationEngine();
        }

        public override void Dispose()
        {
            qmlApplicationEngine.Dispose();
            _coreApplication.Dispose();
            
            base.Dispose();
        }
    }
    
    public abstract class AbstractBaseQmlTests<TTypeToRegister> : BaseQmlEngineTests
    {
        protected MockTypeCreator TypeCreator { get;  private set; }

        readonly List<Type> _registeredTypes = new List<Type>();

        protected AbstractBaseQmlTests()
        {
            TypeCreator = new MockTypeCreator();
            Net.TypeCreator.Current = TypeCreator;
        }

        protected virtual void RegisterType<T>()
        {
            if (_registeredTypes.Contains(typeof(T))) return;
            _registeredTypes.Add(typeof(T));
            Net.Qml.RegisterType<T>("tests");
        }

        protected void RunQmlTest(string instanceId, string componentOnCompletedCode, bool runEvents = false)
        {
            var result = NetTestHelper.RunQml(
                qmlApplicationEngine,
                string.Format(
                    @"
                    import QtQuick 2.0
                    import tests 1.0
                    {0} {{
                        id: {1}
                        property var testQObject: null
                        property var assert: null
                        function runTest() {{
                            {2}
                        }}
                    }}
                    ",
                    typeof(TTypeToRegister).Name,
                    instanceId,
                    componentOnCompletedCode),
                runEvents);
            if (result == false)
            {
                throw new Exception($"Couldn't execute qml: {componentOnCompletedCode}");
            }
        }

        public override void Dispose()
        {
            Net.TypeCreator.Current = null;

            base.Dispose();
        }
    }

    public abstract class BaseQmlTests<T> : AbstractBaseQmlTests<T>
        where T : class
    {
        protected readonly Mock<T> Mock;

        protected BaseQmlTests()
        {
            RegisterType<T>();
            Mock = new Mock<T>();
            TypeCreator.SetInstance(typeof(T), Mock.Object);
        }
    }

    public abstract class BaseQmlTestsWithInstance<T> : AbstractBaseQmlTests<T>
        where T : class, new()
    {
        protected readonly T Instance;

        protected BaseQmlTestsWithInstance()
        {
            RegisterType<T>();
            Instance = new T();
            TypeCreator.SetInstance(typeof(T), Instance);
        }
    }

    public abstract class BaseQmlMvvmTestsWithInstance<T> : AbstractBaseQmlTests<T>
        where T : class, new()
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
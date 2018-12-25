using System;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SingletonTests : BaseQmlTests<SingletonTests.SingletonTestObject>
    {
        public class SingletonTestObject
        {
            public virtual string Property { get; set; }
        }

        [Fact]
        public void Can_register_singleton()
        {
            NetTestHelper.RunQml(
                qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                Item {
                    id: test
                    Component.onCompleted: function() {
                        SingletonTestObject.property = ""test""
                    }
                }");

            Mock.VerifySet(x => x.Property = "test", Times.Once);
        }

        protected override void RegisterType<T>()
        {
            Net.Qml.RegisterSingletonType<T>("tests");
        }
    }
}
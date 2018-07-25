using System;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class JsValueTests : BaseQmlTests<JsValueTests.JsTestsQml>
    {
        public class JsTestsQml
        {
            public virtual void Method(NetJsValue value)
            {

            }
        }

        [Fact]
        public void Can_send_function()
        {
            NetJsValue jsValue = null;
            Mock.Setup(x => x.Method(It.IsAny<NetJsValue>()))
                .Callback(new Action<NetJsValue>(x => jsValue = x));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Method(function(){})
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<NetJsValue>()), Times.Once);
            jsValue.Should().NotBeNull();
            jsValue.IsCallable.Should().BeTrue();
        }
        
        [Fact]
        public void Can_send_non_function()
        {
            NetJsValue jsValue = null;
            Mock.Setup(x => x.Method(It.IsAny<NetJsValue>()))
                .Callback(new Action<NetJsValue>(x => jsValue = x));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Method({})
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<NetJsValue>()), Times.Once);
            jsValue.Should().NotBeNull();
            jsValue.IsCallable.Should().BeFalse();
        }
    }
}
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
            public virtual void Method(INetJsValue value)
            {

            }

            public virtual void MethodWithoutParams()
            {
                
            }
            
            public virtual void MethodWithParameters(string param1, int param2)
            {
                
            }
        }

        [Fact]
        public void Can_send_function()
        {
            INetJsValue jsValue = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<INetJsValue>(x => jsValue = x));
            
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

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Once);
            jsValue.Should().NotBeNull();
            jsValue.IsCallable.Should().BeTrue();
        }
        
        [Fact]
        public void Can_send_non_function()
        {
            INetJsValue jsValue = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<INetJsValue>(x => jsValue = x));
            
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

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Once);
            jsValue.Should().NotBeNull();
            jsValue.IsCallable.Should().BeFalse();
        }

        [Fact]
        public void Can_invoke_js_callback()
        {
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<INetJsValue>(x =>
                {
                    x.Call();
                }));
            Mock.Setup(x => x.MethodWithoutParams());
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Method(function() {
                                test.MethodWithoutParams()
                            })
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Once);
            Mock.Verify(x => x.MethodWithoutParams(), Times.Once);
        }
    }
}
using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class JsValueTests : BaseQmlTests<JsValueTests.JsTestsQml>
    {
        public class JsTestsQml
        {
            public virtual void Method(dynamic value)
            {

            }

            public virtual void MethodWithoutParams()
            {
                
            }
            
            public virtual void MethodWithParameters(string param1, int param2)
            {
                
            }
            
            public virtual void CallMethodWithJsValue(INetJsValue value, INetJsValue method)
            {
                method.Call(value);
            }

            public virtual TestObject GetTestObject()
            {
                return null;
            }
            
            public class TestObject
            {
                public int CalledCount { get; set; }
                
                public void TestMethod()
                {
                    CalledCount++;
                }
            }
        }

        [Fact]
        public void Can_send_function()
        {
            INetJsValue jsValue = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic>(x => jsValue = x));
            
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
                .Callback(new Action<dynamic>(x =>
                {
                    jsValue = x;
                }));
            
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
            object result = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic>(x =>
                {
                    result = x();
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
            result.Should().BeNull();
        }

        [Fact]
        public void Can_invoke_js_callback_with_parameters()
        {
            object result = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic>(x =>
                {
                    result = x("test1", 4);
                }));
            Mock.Setup(x => x.MethodWithParameters("test1", 4));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Method(function(param1, param2) {
                                test.MethodWithParameters(param1, param2)
                            })
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Once);
            Mock.Verify(x => x.MethodWithParameters("test1", 4), Times.Once);
            result.Should().BeNull();
        }

        [Fact]
        public void Can_invoke_js_callback_with_net_instance()
        {
            var testObject = new JsValueTests.JsTestsQml.TestObject();
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic>(x =>
                {
                    x(testObject);
                }));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Method(function(param1) {
                                param1.TestMethod()
                                param1.TestMethod()
                            })
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Once);
            testObject.CalledCount.Should().Be(2);
        }
        
        [Fact]
        public void Can_pass_js_value_to_callback()
        {
            Mock.CallBase = true;
            Mock.Setup(x => x.MethodWithParameters("test1", 4));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var o = {
                                testProperty1: 'test1',
                                testProperty2: 4
                            }
                            test.CallMethodWithJsValue(o,
                                function(passedIn) {
                                    test.MethodWithParameters(passedIn.testProperty1, passedIn.testProperty2)
                                })
                        }
                    }
                ");

            Mock.Verify(x => x.CallMethodWithJsValue(It.IsAny<INetJsValue>(), It.IsAny<INetJsValue>()), Times.Once);
            Mock.Verify(x => x.MethodWithParameters("test1", 4), Times.Once);
        }

        [Fact]
        public void Can_get_return_value_from_js_function()
        {
            var testObject = new JsTestsQml.TestObject();
            var results = new List<object>();
            Mock.Setup(x => x.GetTestObject()).Returns(testObject);
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>())).Callback(new Action<dynamic>(jsValue =>
                {
                    results.Add((object)jsValue());
                }));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var jsObject = {
                            }
                            test.Method(function() {
                                return 'test'
                            })
                            test.Method(function() {
                                return 32
                            })
                            test.Method(function() {
                                return jsObject
                            })
                            test.Method(function() {
                                return function() {}
                            })
                            var netObject = test.GetTestObject()
                            test.Method(function() {
                                return netObject
                            })
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Exactly(5));
            Mock.Verify(x => x.GetTestObject(), Times.Once);
            results.Should().HaveCount(5);
            results[0].Should().Be("test");
            results[1].Should().Be(32);
            results[2].Should().BeAssignableTo<INetJsValue>().And.Subject.As<INetJsValue>().IsCallable.Should().BeFalse();
            results[3].Should().BeAssignableTo<INetJsValue>().And.Subject.As<INetJsValue>().IsCallable.Should().BeTrue();
            results[4].Should().BeSameAs(testObject);
        }

        [Fact]
        public void Can_read_properties_from_js_object()
        {
            var testObject = new JsValueTests.JsTestsQml.TestObject();
            dynamic result = null;
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>())).Callback(new Action<dynamic>(jsValue =>
                {
                    result = jsValue;
                }));
            Mock.Setup(x => x.GetTestObject()).Returns(testObject);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var netObject = test.GetTestObject()
                            test.Method({
                                test1: 34,
                                test2: 'test3',
                                test3: netObject,
                                test4: {
                                    test5: 'test5'
                                }
                            })
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Exactly(1));
            Mock.Verify(x => x.GetTestObject(), Times.Exactly(1));
            ((object) result.nonExistant).Should().BeNull();
            ((int) result.test1).Should().Be(34);
            ((string) result.test2).Should().Be("test3");
            ((object)result.test3).Should().BeSameAs(testObject);
            ((string) result.test4.test5).Should().Be("test5");
        }
    }
}
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

            public virtual void Method(dynamic value1, dynamic value2)
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
                            test.method(function(){})
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
                            test.method({})
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
                            test.method(function() {
                                test.methodWithoutParams()
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
                            test.method(function(param1, param2) {
                                test.methodWithParameters(param1, param2)
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
                            test.method(function(param1) {
                                param1.testMethod()
                                param1.testMethod()
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
                            test.callMethodWithJsValue(o,
                                function(passedIn) {
                                    test.methodWithParameters(passedIn.testProperty1, passedIn.testProperty2)
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
                            test.method(function() {
                                return 'test'
                            })
                            test.method(function() {
                                return 32
                            })
                            test.method(function() {
                                return jsObject
                            })
                            test.method(function() {
                                return function() {}
                            })
                            var netObject = test.getTestObject()
                            test.method(function() {
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
                            var netObject = test.getTestObject()
                            test.method({
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

        [Fact]
        public void Can_set_properties()
        {
            var testObject = new JsValueTests.JsTestsQml.TestObject();
            dynamic result = null;
            Mock.Setup(x => x.GetTestObject()).Returns(testObject);
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>(), It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic, dynamic>((source, destination) =>
                {
                    destination.dest1 = source.source1;
                    destination.dest2 = source.source2;
                    destination.dest3 = source.source3;
                    destination.dest4 = source.source4;
                }));
            Mock.Setup(x => x.Method(It.IsAny<INetJsValue>()))
                .Callback(new Action<dynamic>(value => { result = value; }));
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    JsTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var netObject = test.getTestObject()
                            var source = {
                                source1: 123,
                                source2: 'value',
                                source3: netObject,
                                source4: {
                                }
                            }
                            var destination = {
                            }
                            test.method(source, destination)
                            test.method(destination)
                        }
                    }
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>()), Times.Exactly(1));
            Mock.Verify(x => x.Method(It.IsAny<INetJsValue>(), It.IsAny<INetJsValue>()), Times.Exactly(1));
            Mock.Verify(x => x.GetTestObject(), Times.Exactly(1));
            ((object) result).Should().NotBeNull();
            ((int) result.dest1).Should().Be(123);
            ((string) result.dest2).Should().Be("value");
            ((object) result.dest3).Should().BeSameAs(testObject);
            ((object) result.dest4).Should().BeAssignableTo<INetJsValue>();
        }
    }
}
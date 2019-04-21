using System;
using FluentAssertions;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SignalTests : BaseQmlTests<SignalTests.ObjectTestsQml>
    {
        public class ObjectTestsQml
        {
            public virtual SignalObject GetSignalObject()
            {
                return null;
            }

            public virtual bool SignalRaised { get; set; }

            public virtual void MethodWithArgs(string arg1, int arg2)
            {
            }

            public virtual void TestMethod()
            {
            }

            public virtual void TestMethodWithArgs(string arg1, int arg2)
            {
            }

            private string _someStringPropertyValue = "";

            [NotifySignal]
            public string SomeStringProperty
            {
                get => _someStringPropertyValue;
                set
                {
                    if (_someStringPropertyValue == value)
                        return;
                    _someStringPropertyValue = value;
                    this.ActivateNotifySignal();
                }
            }

            private int _someIntPropertyValue = 0;

            [NotifySignal("someWeirdSignalName")]
            public int SomeIntProperty
            {
                get => _someIntPropertyValue;
                set
                {
                    if (_someIntPropertyValue == value)
                        return;
                    _someIntPropertyValue = value;
                    this.ActivateNotifySignal();
                }
            }

            private bool _someBoolPropertyValue = false;

            public bool SomeBoolProperty
            {
                get
                {
                    return _someBoolPropertyValue;
                }

                set
                {
                    if (_someBoolPropertyValue == value)
                        return;
                    _someBoolPropertyValue = value;
                    this.ActivateNotifySignal();
                }
            }

            public virtual object GetTestObject()
            {
                return null;
            }
        }

        [Signal("testSignal")]
        [Signal("testSignalWithArgs1", NetVariantType.String, NetVariantType.Int)]
        [Signal("testSignalWithArgs2", NetVariantType.String, NetVariantType.Int)]
        [Signal("testSignalWithNetArg", NetVariantType.Object)]
        public class SignalObject
        {
        }

        [Signal("derivedSignal")]
        public class SignalObjectDerived : SignalObject
        {
        }

        [Fact]
        public void Can_raise_signal_from_qml()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.testSignal.connect(function() {
                        test.signalRaised = true
                    })
                    instance.testSignal()
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }

        [Fact]
        public void Can_raise_signal_from_qml_with_args()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.MethodWithArgs("arg1", 3));

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.testSignalWithArgs1.connect(function(arg1, arg2) {
                        test.signalRaised = true
                        test.methodWithArgs(arg1, arg2)
                    })
                    instance.testSignalWithArgs1('arg1', 3)
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
            Mock.Verify(x => x.MethodWithArgs("arg1", 3), Times.Once);
        }

        [Fact(Skip = "This was broken when issue #71 was resolved. If you need this, file an issue.")]
        public void Can_raise_signal_from_qml_different_retrieval_of_net_instance()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    var instance1 = test.getSignalObject()
                    instance1.testSignal.connect(function() {
                        test.signalRaised = true
                    })
                    var instance2 = test.getSignalObject()
                    instance2.testSignal()
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }

        [Fact]
        public void Can_raise_signal_from_net()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                signalObject.ActivateSignal("testSignal");
            });
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    var instance1 = test.getSignalObject()
                    instance1.testSignal.connect(function() {
                        test.signalRaised = true
                    })
                    test.testMethod()
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }

        [Fact]
        public void Can_raise_changed_default_signal_from_net()
        {
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    test.someStringPropertyChanged.connect(function() {
                        test.signalRaised = true
                    })
                    test.someStringProperty = 'NewValue'
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }

        [Fact]
        public void Can_raise_changed_custom_signal_from_net()
        {
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    test.someWeirdSignalName.connect(function() {
                        test.signalRaised = true
                    })
                    test.someIntProperty = 1
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }

        [Fact]
        public void Can_not_raise_invalid_changed_signal_from_net()
        {
            Mock.Setup(x => x.SignalRaised).Returns(false);

            RunQmlTest(
                "test",
                @"
                    test.someBoolPropertyChanged.connect(function() {
                        test.signalRaised = true
                    })
                    test.someBoolProperty = true
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Never);
        }

        [Fact]
        public void Can_raise_signal_from_net_with_args()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                signalObject.ActivateSignal("testSignalWithArgs1", "arg1", 3);
            });
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.MethodWithArgs("arg1", 3));

            RunQmlTest(
                "test",
                @"
                    var instance1 = test.getSignalObject()
                    instance1.testSignalWithArgs1.connect(function(arg1, arg2) {
                        test.signalRaised = true
                        test.methodWithArgs(arg1, arg2)
                    })
                    test.testMethod()
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
            Mock.Verify(x => x.MethodWithArgs("arg1", 3), Times.Once);
        }

        [Fact]
        public void Can_attach_delegate_to_signal_when_object_not_in_qml()
        {
            var o = new SignalObject();
            string message1 = null;
            string message2 = null;

            o.AttachToSignal("testSignalWithArgs1", new Action<string, int>((m, _) => { message1 = m; }));
            o.AttachToSignal("testSignalWithArgs2", new Action<string, int>((m, _) => { message2 = m; }));

            o.ActivateSignal("testSignalWithArgs1", "message1", 3);
            message1.Should().Be("message1");
            message2.Should().BeNull();
            message1 = null;

            o.ActivateSignal("testSignalWithArgs2", "message2", 4);
            message1.Should().BeNull();
            message2.Should().Be("message2");
        }

        [Fact]
        public void Can_raise_net_signal_from_qml_when_added_before_qml()
        {
            var o = new SignalObject();
            string message = null;
            o.AttachToSignal("testSignalWithArgs1", new Action<string, int>((m, _) => message = m));
            Mock.Setup(x => x.GetSignalObject()).Returns(o);

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.testSignalWithArgs1('from qml', 2)
                ");

            message.Should().Be("from qml");
        }

        [Fact]
        public void Can_raise_net_signal_from_qml_when_added_after_qml()
        {
            var o = new SignalObject();
            string message = null;
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.AttachToSignal("testSignalWithArgs1", new Action<string, int>((m, _) => message = m));
            });

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    test.testMethod()
                    instance.testSignalWithArgs1('from qml', 3)
                ");

            message.Should().Be("from qml");
        }

        [Fact]
        public void Can_raise_net_signal_from_qml_with_net_object_parameter()
        {
            var o = new SignalObject();
            var param = new ObjectTestsQml();
            param.SomeStringProperty = Guid.NewGuid().ToString();
            ObjectTestsQml paramResult = null;
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.GetTestObject()).Returns(param);
            o.AttachToSignal("testSignalWithNetArg", new Action<ObjectTestsQml>((p) =>
            {
                paramResult = p;
            }));

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    var param = test.getTestObject()
                    instance.testSignalWithNetArg(param)
                ");

            paramResult.Should().NotBeNull();
            paramResult.SomeStringProperty.Should().NotBeNull(param.SomeStringProperty);
        }

        [Fact]
        public void Can_raise_net_signal_from_net_with_net_object_parameter()
        {
            var o = new SignalObject();
            var param = new ObjectTestsQml();
            param.SomeStringProperty = Guid.NewGuid().ToString();
            ObjectTestsQml paramResult = null;
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.GetTestObject()).Returns(param);
            // When testMethod() is called, activate the signal (from .NET)
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.ActivateSignal("testSignalWithNetArg", param);
            });
            o.AttachToSignal("testSignalWithNetArg", new Action<ObjectTestsQml>((p) =>
            {
                paramResult = p;
            }));

            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    test.testMethod()
                ");

            paramResult.Should().NotBeNull();
            paramResult.SomeStringProperty.Should().NotBeNull(param.SomeStringProperty);
        }

        [Fact]
        public void Can_raise_base_signal_from_net_on_base_object_from_derived_object()
        {
            var o = new SignalObjectDerived();
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.ActivateSignal("testSignal");
            });
            
            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.testSignal.connect(function() {
                        test.signalRaised = true
                    })
                    test.testMethod()
                ");
            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
        
        [Fact]
        public void Can_raise_derived_signal_from_net_on_base_object_from_derived_object()
        {
            var o = new SignalObjectDerived();
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.ActivateSignal("derivedSignal");
            });
            
            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.derivedSignal.connect(function() {
                        test.signalRaised = true
                    })
                    test.testMethod()
                ");
            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
        
        [Fact]
        public void Can_raise_base_signal_from_qml_on_base_object_from_derived_object()
        {
            var o = new SignalObjectDerived();
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.ActivateSignal("testSignal");
            });
            
            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.testSignal.connect(function() {
                        test.signalRaised = true
                    })
                    test.testMethod()
                ");
            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
        
        [Fact]
        public void Can_raise_derived_signal_from_qml_on_base_object_from_derived_object()
        {
            var o = new SignalObjectDerived();
            Mock.Setup(x => x.SignalRaised).Returns(false);
            Mock.Setup(x => x.GetSignalObject()).Returns(o);
            Mock.Setup(x => x.TestMethod()).Callback(() =>
            {
                o.ActivateSignal("derivedSignal");
            });
            
            RunQmlTest(
                "test",
                @"
                    var instance = test.getSignalObject()
                    instance.derivedSignal.connect(function() {
                        test.signalRaised = true
                    })
                    test.testMethod()
                ");
            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
    }
}
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

            private string _somePropertyValue = "";

            [NotifySignal]
            public string SomeProperty
            {
                get => _somePropertyValue;
                set {
                    if (_somePropertyValue == value) 
                        return;
                    _somePropertyValue = value;
                    this.ActivatePropertyChangedSignal();
                }
            }
        }

        [Signal("testSignal")]
        [Signal("testSignalWithArgs1", NetVariantType.String, NetVariantType.Int)]
        [Signal("testSignalWithArgs2", NetVariantType.String, NetVariantType.Int)]
        public class SignalObject
        {
            
        }

        [Fact]
        public void Can_raise_signal_from_qml()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.SignalRaised).Returns(false);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance = test.getSignalObject()
                            instance.testSignal.connect(function() {
                                test.signalRaised = true
                            })
                            instance.testSignal()
                        }
                    }
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
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance = test.getSignalObject()
                            instance.testSignalWithArgs1.connect(function(arg1, arg2) {
                                test.signalRaised = true
                                test.methodWithArgs(arg1, arg2)
                            })
                            instance.testSignalWithArgs1('arg1', 3)
                        }
                    }
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
            Mock.Verify(x => x.MethodWithArgs("arg1", 3), Times.Once);
        }
        
        [Fact]
        public void Can_raise_signal_from_qml_different_retrieval_of_net_instance()
        {
            var signalObject = new SignalObject();
            Mock.Setup(x => x.GetSignalObject()).Returns(signalObject);
            Mock.Setup(x => x.SignalRaised).Returns(false);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.getSignalObject()
                            instance1.testSignal.connect(function() {
                                test.signalRaised = true
                            })
                            var instance2 = test.getSignalObject()
                            instance2.testSignal()
                        }
                    }
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
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.getSignalObject()
                            instance1.testSignal.connect(function() {
                                test.signalRaised = true
                            })
                            test.testMethod()
                        }
                    }
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
        
        [Fact]
        public void Can_raise_changed_signal_from_net()
        {
            Mock.Setup(x => x.SignalRaised).Returns(false);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.somePropertyChanged.connect(function() {
                                test.signalRaised = true
                            })
                            test.someProperty = 'NewValue'
                        }
                    }
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
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
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.getSignalObject()
                            instance1.testSignalWithArgs1.connect(function(arg1, arg2) {
                                test.signalRaised = true
                                test.methodWithArgs(arg1, arg2)
                            })
                            test.testMethod()
                        }
                    }
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
            o.AttachToSignal("testSignalWithArgs2", new Action<string,int>((m, _) => { message2 = m; }));
            
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
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance = test.getSignalObject()
                            instance.testSignalWithArgs1('from qml', 2)
                        }
                    }
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
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    ObjectTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance = test.getSignalObject()
                            test.testMethod()
                            instance.testSignalWithArgs1('from qml', 3)
                        }
                    }
                ");

            message.Should().Be("from qml");
        }
    }
}
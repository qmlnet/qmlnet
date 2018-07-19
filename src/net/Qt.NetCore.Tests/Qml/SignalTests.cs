using System;
using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests.Qml
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
        }

        [Signal("testSignal")]
        public class SignalObject
        {
            
        }

        [Fact]
        public void Can_raise_signal()
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
                            var instance = test.GetSignalObject()
                            instance.testSignal.connect(function() {
                                test.SignalRaised = true
                            })
                            instance.testSignal()
                        }
                    }
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
        
        [Fact]
        public void Can_raise_signal_from_different_retrieval_of_net_instance()
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
                            var instance1 = test.GetSignalObject()
                            instance1.testSignal.connect(function() {
                                test.SignalRaised = true
                            })
                            var instance2 = test.GetSignalObject()
                            instance2.testSignal()
                        }
                    }
                ");

            Mock.VerifySet(x => x.SignalRaised = true, Times.Once);
        }
    }
}
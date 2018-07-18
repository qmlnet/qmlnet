using Qt.NetCore.Qml;
using System;
using Xunit;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Qt.NetCore.Tests
{
    public class NetInstanceInteropTests : BaseQmlTestsWithInstance<NetInstanceInteropTests.NetInteropTestQml>
    {
        public class SecondLevelType
        {
            public bool IsSame(SecondLevelType other)
            {
                return ReferenceEquals(this, other);
            }
        }

        public class NetInteropTestQml
        {
            public NetInteropTestQml()
            {
                Parameter = new SecondLevelType();
                Parameter2 = new SecondLevelType();
                _parameterWeakRef = new WeakReference<SecondLevelType>(Parameter);
            }

            public SecondLevelType Parameter { get; set; }
            
            public SecondLevelType Parameter2 { get; set; }

            private readonly WeakReference<SecondLevelType> _parameterWeakRef;

            public void ReleaseNetReferenceParameter()
            {
                Parameter = null;
            }
            
            public bool CheckIsParameterAlive()
            {
                GC.Collect(GC.MaxGeneration);
                return _parameterWeakRef.TryGetTarget(out SecondLevelType _);
            }

            public bool TestResult { get; set; }
        }

        [Fact]
        public void Can_handle_multiple_instances_equality_qml()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    NetInteropTestQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.Parameter;
                            var instance2 = test.Parameter;

                            test.TestResult = instance1.IsSame(instance2);
                        }
                    }
                ");
            Assert.True(Instance.TestResult);
        }

        [Fact]
        public void Can_handle_different_instances_equality_qml()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    NetInteropTestQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.Parameter;
                            var instance2 = test.Parameter2;

                            test.TestResult = instance1.IsSame(instance2);
                        }
                    }
                ");
            Assert.False(Instance.TestResult);
        }

        [Fact]
        public void Can_handle_instance_deref_of_one_ref_in_qml()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    NetInteropTestQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.Parameter;
                            var instance2 = test.Parameter;

                            //deref Parameter
                            instance2 = null;
                            
                            gc();

                            test.TestResult = test.CheckIsParameterAlive();
                        }
                    }
                ");
            Assert.True(Instance.TestResult);
        }

        [Fact]
        public void Can_handle_instance_deref_of_all_refs_in_qml()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    NetInteropTestQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.Parameter;
                            var instance2 = test.Parameter;

                            //deref Parameter
                            instance1 = null;
                            instance2 = null;
                            
                            gc();

                            test.TestResult = test.CheckIsParameterAlive();
                        }
                    }
                ");
            GC.Collect(GC.MaxGeneration);
            Assert.True(Instance.TestResult);
        }

        [Fact(Skip = "Not working yet")]
        public void Can_handle_instance_deref_of_all_refs_in_qml_and_net()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    NetInteropTestQml {
                        id: test
                        Component.onCompleted: function() {
                            var instance1 = test.Parameter;
                            var instance2 = test.Parameter;

                            //deref Parameter
                            instance1 = null;
                            instance2 = null;
                            test.ReleaseNetReferenceParameter();
                            
                            gc();
gc();
gc();
gc();

                            test.TestResult = test.CheckIsParameterAlive();
                        }
                    }
                ");
            GC.Collect(GC.MaxGeneration);
            Assert.False(Instance.TestResult);
        }
    }
}

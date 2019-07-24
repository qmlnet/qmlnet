using System;
using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class LifetimeTests : BaseQmlTestsWithInstance<LifetimeTests.NetInteropTestQml>
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

            public void CollectGc()
            {
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();
            }
            
            public SecondLevelType Parameter { get; set; }

            public SecondLevelType Parameter2 { get; set; }

            private readonly WeakReference<SecondLevelType> _parameterWeakRef;

            public void ReleaseNetReferenceParameter()
            {
                Parameter = null;
                Parameter2 = null;
            }

            public bool CheckIsParameterAlive()
            {
                GC.Collect(GC.MaxGeneration);
                return _parameterWeakRef.TryGetTarget(out SecondLevelType _);
            }

            public bool? TestResult { get; set; }
        }

        [Fact]
        public void Can_handle_multiple_instances_equality_qml()
        {
            RunQmlTest("test",
                @"
                    var instance1 = test.parameter
                    var instance2 = test.parameter

                    test.testResult = instance1.isSame(instance2)
                ");
            
            Assert.True(Instance.TestResult);
        }

        [Fact]
        public void Can_handle_different_instances_equality_qml()
        {
            RunQmlTest("test",
                @"
                    var instance1 = test.parameter;
                    var instance2 = test.parameter2;

                    test.testResult = instance1.isSame(instance2);
                ");
            
            Assert.False(Instance.TestResult);
        }

        [Fact]
        public void Can_handle_instance_deref_of_one_ref_in_qml()
        {
            RunQmlTest("test",
                @"
                    var instance1 = test.parameter;
                    var instance2 = test.parameter;

                    //deref Parameter
                    instance2 = null;
                       
                    Qt.callLater(function() {
                        test.testResult = test.checkIsParameterAlive();
                    })
                ", true);

            Instance.TestResult.Should().BeTrue();
        }

        [Fact]
        public void Can_handle_instance_deref_of_all_refs_in_qml()
        {
            RunQmlTest("test",
                @"
                    var instance = test.parameter;
                    instance = null;

                    gc()

                    Qt.callLater(function() {
                        test.releaseNetReferenceParameter()
                        test.collectGc()
                        test.testResult = test.checkIsParameterAlive();
                    });
                ", true);

            Instance.TestResult.Should().BeFalse();   
        }
        
        [Fact]
        public void Can_handle_instance_ref_of_all_refs_in_qml()
        {
            RunQmlTest("test",
                @"
                    var instance = test.parameter;

                    gc()

                    Qt.callLater(function() {
                        test.releaseNetReferenceParameter()
                        test.collectGc()
                        test.testResult = test.checkIsParameterAlive();
                    });
                ", true);

            Instance.TestResult.Should().BeTrue(); 
        }

        [Fact]
        public void Can_handle_deleting_one_qml_ref_does_not_release()
        {
            RunQmlTest("test",
                @"
                    var instance1 = test.parameter;
                    var instance2 = test.parameter;

                    instance1 = null;

                    gc()

                    Qt.callLater(function() {
                        test.releaseNetReferenceParameter()
                        test.collectGc()
                        test.testResult = test.checkIsParameterAlive();
                    });
                ", true);

            Instance.TestResult.Should().BeTrue();   
        }
    }
}
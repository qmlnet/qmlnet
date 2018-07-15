using System;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeManagerTests : BaseTests
    {
        public class TestType1
        {
            public void TestMethod()
            {
                
            }
        }
        
        [Fact]
        public void Can_get_net_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType1>();

            typeInfo.FullTypeName.Should().Be(typeof(TestType1).AssemblyQualifiedName);
            typeInfo.ClassName.Should().Be("TestType1");
            typeInfo.PrefVariantType.Should().Be(NetVariantType.Object);
            typeInfo.MethodCount.Should().Be(1);
        }

        public class TestType2
        {
            public void TestMethod(int parameter1, TestType1 parameter2)
            {
                
            }
        }
        
        [Fact]
        public void Can_get_method_parameters()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType2>();

            typeInfo.MethodCount.Should().Be(1);
            
            var method = typeInfo.GetMethod(0);
            method.Should().NotBeNull();
            method.ParameterCount.Should().Be(2);
            
            var parameter1 = method.GetParameter(0);
            parameter1.Should().NotBeNull();
            parameter1.Name.Should().Be("parameter1");
            var parameter1Type = parameter1.Type;
            parameter1Type.Should().NotBeNull();
            parameter1Type.ClassName.Should().Be("Int32");
            
            var parameter2 = method.GetParameter(1);
            parameter2.Should().NotBeNull();
            parameter2.Name.Should().Be("parameter2");
            var parameter2Type = parameter2.Type;
            parameter2Type.Should().NotBeNull();
            parameter2Type.ClassName.Should().Be("TestType1");
        }

        public class TestType3
        {
            public TestType3 TestMethod()
            {
                return null;
            }
        }

        [Fact]
        public void Can_get_method_with_return_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestType3>();
            var returnType = type.GetMethod(0).ReturnType;
            returnType.Should().NotBeNull();
            returnType.ClassName.Should().Be("TestType3");
        }

        public class TestType4
        {
            public void TestMethod()
            {
                
            }
        }
        
        [Fact]
        public void Can_get_method_without_return_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestType4>();
            var returnType = type.GetMethod(0).ReturnType;
            returnType.Should().BeNull();
        }

        public class TestType5
        {
            public void ThisIsMethodName()
            {
                
            }
        }

        [Fact]
        public void Can_get_method_name()
        {
            var type = NetTypeManager.GetTypeInfo<TestType5>();
            var method = type.GetMethod(0);
            method.MethodName.Should().Be("ThisIsMethodName");
        }

        [Fact]
        public void Null_type_returned_for_invalid_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo("tt");
            typeInfo.Should().BeNull();
        }
    }
}
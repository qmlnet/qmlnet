using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.Types
{
    public class NetTypeManagerTests : BaseTests
    {
        public class TestType1
        {
            public void TestMethod()
            {
            }

            public int TestProperty { get; set; }
        }

        [Fact]
        public void Can_get_net_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType1>();
            typeInfo.EnsureLoaded();

            typeInfo.FullTypeName.Should().Be(typeof(TestType1).AssemblyQualifiedName);
            typeInfo.ClassName.Should().Be("TestType1");
            typeInfo.PrefVariantType.Should().Be(NetVariantType.Object);
            typeInfo.MethodCount.Should().Be(1);
            typeInfo.PropertyCount.Should().Be(1);
        }

        [Fact]
        public void Can_get_parent_type_on_method()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType1>();
            typeInfo.EnsureLoaded();

            var method = typeInfo.GetMethod(0);
            method.MethodName.Should().Be("TestMethod");
            method.ParentType.FullTypeName.Should().Be(typeof(TestType1).AssemblyQualifiedName);
        }

        [Fact]
        public void Can_get_parent_type_on_property()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType1>();
            typeInfo.EnsureLoaded();

            var property = typeInfo.GetProperty(0);
            property.Name.Should().Be("TestProperty");
            property.ParentType.FullTypeName.Should().Be(typeof(TestType1).AssemblyQualifiedName);
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
            typeInfo.EnsureLoaded();

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
            type.EnsureLoaded();
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
            type.EnsureLoaded();
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
            type.EnsureLoaded();
            var method = type.GetMethod(0);
            method.MethodName.Should().Be("ThisIsMethodName");
        }

        public class TestType6
        {
            public string Property { get; set; }
        }

        [Fact]
        public void Can_get_property()
        {
            var type = NetTypeManager.GetTypeInfo<TestType6>();
            type.EnsureLoaded();
            type.PropertyCount.Should().Be(1);
            var property = type.GetProperty(0);
            property.Should().NotBeNull();
            property.Name.Should().Be("Property");
            property.CanRead.Should().BeTrue();
            property.CanWrite.Should().BeTrue();
            property.ReturnType.ClassName.Should().Be("String");
            property.ParentType.ClassName.Should().Be("TestType6");
            property.NotifySignal.Should().BeNull();
        }

        [Signal("testSignal", NetVariantType.DateTime, NetVariantType.Object)]
        public class TestType7
        {
        }

        [Fact]
        public void Can_get_signal()
        {
            var type = NetTypeManager.GetTypeInfo<TestType7>();
            type.EnsureLoaded();
            type.SignalCount.Should().Be(1);
            var signal = type.GetSignal(0);
            signal.Name.Should().Be("testSignal");
            signal.ParameterCount.Should().Be(2);
            signal.GetParameter(0).Should().Be(NetVariantType.DateTime);
            signal.GetParameter(1).Should().Be(NetVariantType.Object);
        }

        [Signal("signalName")]
        public class TestType8
        {
            [NotifySignal("signalName")]
            public string Property { get; set; }
        }

        [Fact]
        public void Can_get_notifiy_signal_for_property()
        {
            var type = NetTypeManager.GetTypeInfo<TestType8>();
            type.EnsureLoaded();
            type.SignalCount.Should().Be(1);
            type.PropertyCount.Should().Be(1);
            type.GetProperty(0).Name.Should().Be("Property");
            type.GetProperty(0).NotifySignal.Should().NotBeNull();
            type.GetProperty(0).NotifySignal.Name.Should().Be("signalName");
        }

        public class TestType9
        {
            [NotifySignal("signalName")]
            public string Property { get; set; }
        }

        [Fact]
        public void Can_auto_add_signal_for_property_notification()
        {
            var type = NetTypeManager.GetTypeInfo<TestType9>();
            type.EnsureLoaded();
            type.SignalCount.Should().Be(1);
            type.PropertyCount.Should().Be(1);
            type.GetProperty(0).Name.Should().Be("Property");
            type.GetProperty(0).NotifySignal.Should().NotBeNull();
            type.GetProperty(0).NotifySignal.Name.Should().Be("signalName");
        }

        public class TestType10
        {
            [NotifySignal]
            public string Property { get; set; }
        }

        [Fact]
        public void Can_auto_create_signal_with_property_name_if_no_name_given()
        {
            var type = NetTypeManager.GetTypeInfo<TestType10>();
            type.EnsureLoaded();
            type.SignalCount.Should().Be(1);
            type.PropertyCount.Should().Be(1);
            type.GetProperty(0).Name.Should().Be("Property");
            type.GetProperty(0).NotifySignal.Should().NotBeNull();
            type.GetProperty(0).NotifySignal.Name.Should().Be("propertyChanged");
        }

        public class TestType11
        {
            public InnerType TestFunction()
            {
                return null;
            }

            public class InnerType
            {
            }
        }

        [Fact]
        public void Can_lazy_load_types()
        {
            var type = NetTypeManager.GetTypeInfo<TestType11>();
            type.IsLoaded.Should().BeFalse();
            type.EnsureLoaded();
            type.MethodCount.Should().Be(1);
            type.IsLoaded.Should().BeTrue();
            var method = type.GetMethod(0);
            method.Should().NotBeNull();
            method.ReturnType.IsLoaded.Should().BeFalse();
            method.ReturnType.EnsureLoaded();
            method.ReturnType.MethodCount.Should().Be(0);
            method.ReturnType.IsLoaded.Should().BeTrue();
        }

        public class TestType12
        {
            public void LocalMethod()
            {
            }

            public static void StaticMethod()
            {
            }
        }

        [Fact]
        public void Can_detect_static_methods()
        {
            var type = NetTypeManager.GetTypeInfo<TestType12>();
            type.EnsureLoaded();
            type.MethodCount.Should().Be(2);
            type.LocalMethodCount.Should().Be(1);
            type.GetLocalMethod(0).MethodName.Should().Be("LocalMethod");
            type.GetStaticMethod(0).MethodName.Should().Be("StaticMethod");
        }

        public class TestType13
        {
            public string[] Property { get; set; }
        }

        [Fact]
        public void Can_detect_array_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestType13>();
            type.EnsureLoaded();
            var property = type.GetProperty(0);
            var returnType = property.ReturnType;
            returnType.EnsureLoaded();
            returnType.IsArray.Should().BeTrue();
            returnType.IsList.Should().BeFalse();
        }

        public class TestType14
        {
            public List<int> Prop1 { get; set; }

            public System.Collections.ArrayList Prop2 { get; set; }
        }

        [Fact]
        public void Can_detect_list_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestType14>();
            type.EnsureLoaded();
            for (var x = 0; x < type.PropertyCount; x++)
            {
                var property = type.GetProperty(x);
                if (property.Name == "Prop1" || property.Name == "Prop2")
                {
                    property.ReturnType.EnsureLoaded();
                    property.ReturnType.IsList.Should().BeTrue();
                    property.ReturnType.IsArray.Should().BeFalse();
                }
            }
        }

        public class TestType15
        {
        }

        public class TestType16 : TestType15
        {
        }

        [Fact]
        public void Can_detect_base_class()
        {
            var type1 = NetTypeManager.GetTypeInfo<TestType15>();
            type1.BaseType.Should().StartWith("System.Object, ");
            var type2 = NetTypeManager.GetTypeInfo<TestType16>();
            type2.BaseType.Should().StartWith("Qml.Net.Tests.Types.NetTypeManagerTests+TestType15, ");
            var type3 = NetTypeManager.GetTypeInfo<object>();
            type3.BaseType.Should().BeNull();
        }

        public class TestType17
        {
            public void Method1()
            {
            }

            public void Method2()
            {
            }
        }

        [Fact]
        public void Can_get_unique_id_for_method()
        {
            var type = NetTypeManager.GetTypeInfo<TestType17>();
            type.EnsureLoaded();

            var methods = new List<NetMethodInfo>();
            for (var x = 0; x < type.LocalMethodCount; x++)
            {
                methods.Add(type.GetMethod(x));
            }

            methods.Select(x => x.Id).Distinct().Count().Should().Be(2);
        }

        public class TestType18
        {
            public bool Prop1 { get; set; }

            public bool Prop2 { get; set; }
        }

        [Fact]
        public void Can_get_unique_id_for_property()
        {
            var type = NetTypeManager.GetTypeInfo<TestType18>();
            type.EnsureLoaded();

            var properties = new List<NetPropertyInfo>();
            for (var x = 0; x < type.PropertyCount; x++)
            {
                properties.Add(type.GetProperty(x));
            }

            properties.Select(x => x.Id).Distinct().Count().Should().Be(2);
        }

        [Fact]
        public void Can_get_unique_id_for_types()
        {
            var type1 = NetTypeManager.GetTypeInfo<TestType16>();
            var type2 = NetTypeManager.GetTypeInfo<TestType17>();

            type1.Id.Should().NotBe(type2.Id);
        }

        public class TestType19
        {
            public object this[int index]
            {
                get => null;
                set { }
            }
        }

        [Fact]
        public void Can_get_index_parameters()
        {
            var type = NetTypeManager.GetTypeInfo<TestType19>();
            type.EnsureLoaded();

            var prop = type.GetProperty(0);
            prop.Name.Should().Be("Item");

            var indexParameters = prop.GetAllIndexParameters();
            indexParameters.Count.Should().Be(1);
            indexParameters[0].Name.Should().Be("index");
            indexParameters[0].Type.FullTypeName.Should().Be(typeof(int).AssemblyQualifiedName);
        }
    }
}
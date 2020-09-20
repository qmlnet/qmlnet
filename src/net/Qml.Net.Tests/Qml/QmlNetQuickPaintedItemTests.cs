using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class QmlNetQuickPaintedItemTests : BaseQmlQuickPaintedItemTests<QmlNetQuickPaintedItemTests.TestPaintedItem>
    {
        public class TestPaintedItem : QmlNetQuickPaintedItem
        {
            public TestPaintedItem()
            {
                
            }

            public virtual string SomeProperty { get; set; }

            public object QmlValue { get; set; }

            public override void Paint(NetQPainter painter)
            {
            }
        }
        
        [Fact]
        public void Can_get_and_set_additional_properties()
        {
            Mock.SetupGet(x => x.SomeProperty).Returns("Some property value");
            
            RunQmlTest(
                "test",
                @"
                    test.someProperty = test.someProperty
                ");

            Mock.VerifyGet(x => x.SomeProperty, Times.Once);
            Mock.VerifySet(x => x.SomeProperty = "Some property value");
        }
    }
    
    public class QmlNetQuickPaintedItemInstanceTests : BaseQmlQuickPaintedItemTestsWithInstance<QmlNetQuickPaintedItemTests.TestPaintedItem>
    {
        public class TestPaintedItem : QmlNetQuickPaintedItem
        {
            public TestPaintedItem()
            {
                
            }

            public virtual string SomeProperty { get; set; }

            public object QmlValue { get; set; }

            public override void Paint(NetQPainter painter)
            {
            }
        }
        
        [Fact]
        public void Do_quickitem_properties_exist()
        {
            RunQmlTest(
                "test",
                @"
                    test.height = 20;
                    test.qmlValue = test.height;
                ");

            Assert.NotNull(Instance.QmlValue);
            Assert.Equal(20d, Instance.QmlValue);
        }
    }
    
    public class QmlNetQuickPaintedItemTwoLevelClassHierarchyTests : BaseQmlQuickPaintedItemTests<QmlNetQuickPaintedItemTwoLevelClassHierarchyTests.TestPaintedItem>
    {
        public class TestPaintedItem : TestPaintedItemBase
        {
            public virtual string SomeProperty { get; set; }
        }

        public class TestPaintedItemBase : QmlNetQuickPaintedItem
        {
            public virtual string SomeBaseProperty { get; set; }

            public override void Paint(NetQPainter painter)
            {
            }
        }
        
        [Fact]
        public void Can_get_and_set_additional_properties()
        {
            Mock.SetupGet(x => x.SomeProperty).Returns("Some property value");
            Mock.SetupGet(x => x.SomeBaseProperty).Returns("Some base property value");
            
            RunQmlTest(
                "test",
                @"
                    test.someProperty = test.someProperty
                    test.someBaseProperty = test.someBaseProperty
                ");

            Mock.VerifyGet(x => x.SomeProperty, Times.Once);
            Mock.VerifyGet(x => x.SomeBaseProperty, Times.Once);
            Mock.VerifySet(x => x.SomeProperty = "Some property value");
            Mock.VerifySet(x => x.SomeBaseProperty = "Some base property value");
        }
    }
}
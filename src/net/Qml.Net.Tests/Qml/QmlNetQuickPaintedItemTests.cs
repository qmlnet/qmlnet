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
}
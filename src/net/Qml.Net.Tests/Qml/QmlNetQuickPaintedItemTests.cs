using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

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
    
    public class QmlNetQuickPaintedItemInstanceTests : BaseQmlQuickPaintedItemTestsWithInstance<QmlNetQuickPaintedItemInstanceTests.TestPaintedItem>
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
    
    public class QmlNetQuickPaintedItemRenderingTests : BaseQmlQuickPaintedItemTestsWithInstance<QmlNetQuickPaintedItemRenderingTests.TestPaintedItem>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public QmlNetQuickPaintedItemRenderingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public class TestPaintedItem : QmlNetQuickPaintedItem
        {
            private byte[] _imgData;

            public byte[] ImageData => _imgData;
            
            public TestPaintedItem()
            {
                
            }
            
            public override void Paint(NetQPainter painter)
            {
                foreach (var paintAction in _paintActions)
                {
                    paintAction(painter);
                }
            }

            public void AddPaintAction(Action<NetQPainter> paintAction)
            {
                _paintActions.Add(paintAction);
            }

            private List<Action<NetQPainter>> _paintActions = new List<Action<NetQPainter>>();

            public void DoStoreImageData()
            {
                _imgData = PaintToImage("bmp");
            }
        }

        Image<Rgba32> RunQmlRendering(params Action<NetQPainter>[] paintActions)
        {
            foreach (var pa in paintActions)
            {
                Instance.AddPaintAction(pa);
            }
            RunQmlTest(
                "test",
                @"
                    test.doStoreImageData();
                ",
                additionalProperties: "height: 200" + Environment.NewLine + "width:300" + Environment.NewLine + "fillColor:'#FFFFFFFF'");
            
            return Image.Load<Rgba32>(Instance.ImageData);
        }
        
        [Fact]
        public void FillRect_works()
        {
            var img = RunQmlRendering((p) =>
            {
                p.FillRect(0, 0, 150, 100, "#FF0000");
            });

            var red = new Rgba32(0xFF, 0x00, 0x00);
            var white = new Rgba32(0xFF, 0xFF, 0xFF);

            Assert.Equal(white, img[299, 199]);
            
            Assert.Equal(red, img[0, 0]);
            Assert.Equal(red, img[149, 0]);
            Assert.Equal(red, img[0, 99]);
            Assert.Equal(red, img[149, 99]);
            Assert.Equal(white, img[150, 0]);
            Assert.Equal(white, img[149, 100]);
            Assert.Equal(white, img[150, 99]);
        }
        
        [Fact]
        public void FillRectWithImplicitColor_works()
        {
            var img = RunQmlRendering((p) =>
            {
                p.SetBrush("#FF0000");
                p.FillRect(0, 0, 150, 100);
            });

            var red = new Rgba32(0xFF, 0x00, 0x00);
            var white = new Rgba32(0xFF, 0xFF, 0xFF);

            Assert.Equal(white, img[299, 199]);
            
            Assert.Equal(red, img[0, 0]);
            Assert.Equal(red, img[149, 0]);
            Assert.Equal(red, img[0, 99]);
            Assert.Equal(red, img[149, 99]);
            Assert.Equal(white, img[150, 0]);
            Assert.Equal(white, img[149, 100]);
            Assert.Equal(white, img[150, 99]);
        }

        [Fact]
        public void DrawRect_works()
        {
            var img = RunQmlRendering((p) =>
            {
                p.SetPen("#00FF00", 1);
                p.DrawRect(0, 0, 150, 100);
            });

            var green = new Rgba32(0x00, 0xFF, 0x00);
            var white = new Rgba32(0xFF, 0xFF, 0xFF);
            
            Assert.Equal(white, img[299, 199]);
            
            Assert.Equal(green, img[0, 0]);
            Assert.Equal(green, img[149, 0]);
            Assert.Equal(white, img[151, 0]);
            Assert.Equal(white, img[148, 5]);
            Assert.Equal(green, img[0, 99]);
            Assert.Equal(white, img[5, 98]);
            Assert.Equal(green, img[150, 100]);
            Assert.Equal(white, img[151, 0]);
            Assert.Equal(white, img[149, 101]);
            Assert.Equal(white, img[151, 100]);
        }

        [Fact]
        public void DrawArc_works()
        {
            var img = RunQmlRendering((p) =>
            {
                p.SetPen("#00FF00", 1);
                p.DrawArc(0, 0, 150, 100, 0, 45 * 16);
            });

            var green = new Rgba32(0x00, 0xFF, 0x00);
            var white = new Rgba32(0xFF, 0xFF, 0xFF);
            
            Assert.Equal(green, img[128, 14]);
            Assert.Equal(white, img[129, 14]);
            Assert.Equal(white, img[127, 14]);
            Assert.Equal(white, img[128, 15]);
            Assert.Equal(white, img[128, 13]);
            
            Assert.Equal(green, img[129, 15]);
            
            Assert.Equal(green, img[130, 16]);
            
            Assert.Equal(green, img[131, 17]);
            Assert.Equal(green, img[132, 17]);
            
            Assert.Equal(green, img[150, 50]);
            Assert.Equal(white, img[151, 50]);
            Assert.Equal(white, img[149, 50]);
            Assert.Equal(white, img[150, 51]);
            Assert.Equal(white, img[150, 49]);
        }
        
        [Fact]
        public void DrawConvexPolygon_works()
        {
            var img = RunQmlRendering((p) =>
            {
                p.SetPen("#00FF00", 1);
                p.DrawConvexPolygon(new[]
                {
                    new Point(10, 10),
                    new Point(10, 100),
                    new Point(100, 100),
                    new Point(100, 10),
                    new Point(10, 10)
                });
            });

            var green = new Rgba32(0x00, 0xFF, 0x00);
            var white = new Rgba32(0xFF, 0xFF, 0xFF);
            
            Assert.Equal(green, img[10, 10]);
            Assert.Equal(green, img[10, 50]);
            Assert.Equal(green, img[10, 100]);
            Assert.Equal(green, img[50, 100]);
            Assert.Equal(green, img[100, 100]);
            Assert.Equal(green, img[100, 50]);
            Assert.Equal(green, img[100, 10]);
            Assert.Equal(green, img[50, 10]);
            
            Assert.Equal(white, img[8, 10]);
            Assert.Equal(white, img[12, 12]);
            Assert.Equal(white, img[8, 100]);
            
            Assert.Equal(white, img[100, 8]);
            Assert.Equal(white, img[102, 10]);
            Assert.Equal(white, img[102, 100]);
            Assert.Equal(white, img[50, 102]);
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
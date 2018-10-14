using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ListModelTests : BaseQmlTests<ListModelTests.ListModelTestsQml>
    {
        public class ListModelTestsQml
        {
            public virtual List<TestNetObject> GetNetObjectList()
            {
                return null;
            }

            public virtual void Test(object param)
            {
                
            }
        }

        public class TestNetObject
        {
            public TestNetObject()
            {
                Prop = Guid.NewGuid().ToString();
            }
            
            public string Prop { get; set; }
        }

        [Fact]
        public void Can_get_items_from_list_model()
        {
            var list = new List<TestNetObject>();
            list.Add(new TestNetObject());
            list.Add(new TestNetObject());
            list.Add(new TestNetObject());
            var result = new List<TestNetObject>();
            Mock.Setup(x => x.GetNetObjectList()).Returns(list);
            Mock.Setup(x => x.Test(It.IsAny<object>())).Callback(new Action<object>(o => result.Add((TestNetObject)o)));

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    Item {
                        ListModelTestsQml {
                            id: test
                            Component.onCompleted: function() {
                                var list = test.getNetObjectList()
                                var listModel = Net.toListModel(list)
                                rep.model = listModel
                            }
                        }
                        Repeater {
                            id: rep
                            Item {
                                Component.onCompleted: {
                                    test.test(modelData)
                                }
                            }
                        }
                    }
                ");
            
            Mock.Verify(x => x.GetNetObjectList(),Times.Once);
            Mock.Verify(x => x.Test(It.IsAny<object>()), Times.Exactly(3));
            list.Count.Should().Be(result.Count);
            list[0].Prop.Should().Be(result[0].Prop);
            list[1].Prop.Should().Be(result[1].Prop);
            list[2].Prop.Should().Be(result[2].Prop);
        }

        [Fact]
        public void Can_get_length()
        {
            var list = new List<TestNetObject>();
            list.Add(new TestNetObject());
            Mock.Setup(x => x.GetNetObjectList()).Returns(list);
            
            RunQmlTest(
                "test",
                @"
                    var list = test.getNetObjectList()
                    var listModel = Net.toListModel(list)
                    test.test(listModel.length());
                ");

            Mock.Verify(x => x.Test(1), Times.Once);
        }
        
        [Fact]
        public void Can_get_at()
        {
            var list = new List<TestNetObject>();
            list.Add(new TestNetObject());
            Mock.Setup(x => x.GetNetObjectList()).Returns(list);
            
            RunQmlTest(
                "test",
                @"
                    var list = test.getNetObjectList()
                    var listModel = Net.toListModel(list)
                    test.test(listModel.at(0));
                ");

            Mock.Verify(x => x.Test(list[0]), Times.Once);
        }
        
        [Fact]
        public void Can_get_at_invalid()
        {
            var list = new List<TestNetObject>();
            list.Add(new TestNetObject());
            Mock.Setup(x => x.GetNetObjectList()).Returns(list);
            
            RunQmlTest(
                "test",
                @"
                    var list = test.getNetObjectList()
                    var listModel = Net.toListModel(list)
                    test.test(listModel.at(-1));
                    test.test(listModel.at(1));
                ");

            Mock.Verify(x => x.Test(null), Times.Exactly(2));
        }
    }
}
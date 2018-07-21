using Qml.Net.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Threading;

namespace Qml.Net.Tests.Internal
{
    public class ObjectTaggerTests : BaseTests
    {
        private static void SetMaxIdTo(UInt64 maxId)
        {
            var fieldInfo = typeof(ObjectId)
                                .GetField("MaxId", BindingFlags.NonPublic | BindingFlags.Static);
            fieldInfo.SetValue(null, maxId);
        }

        private static void Reset()
        {
            var methodInfo = typeof(ObjectId)
                                .GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo.Invoke(null, new object[0]);
        }

        public ObjectTaggerTests()
        {
            Reset();
        }

        public override void Dispose()
        {
            Reset();
            base.Dispose();
        }

        [Fact]
        void Can_detect_id_overflow()
        {
            SetMaxIdTo(10);
            List<object> handledObjects = new List<object>();
            for(int i=0; i < 9; i++)
            {
                var obj = new object();
                obj.GetOrCreateTag();
                handledObjects.Add(obj);
            }
            var lastObj = new object();
            Assert.Throws<Exception>(() => { lastObj.GetOrCreateTag(); });
        }

        [Fact]
        void Can_handle_id_overflow_by_filling_spots()
        {
            SetMaxIdTo(10);
            List<object> handledObjects = new List<object>();
            for (int i = 0; i < 9; i++)
            {
                var obj = new object();
                obj.GetOrCreateTag();
                handledObjects.Add(obj);
            }
            handledObjects.RemoveAt(4);
            handledObjects.RemoveAt(3);
            GC.Collect(2, GCCollectionMode.Forced, true);
            Thread.Sleep(100);
            var obj10 = new object();
            var tag10 = obj10.GetOrCreateTag();
            tag10.Should().Be(10ul);
            var obj4 = new object();
            var tag4 = obj4.GetOrCreateTag();
            tag4.Should().Be(4ul);
        }

        [Fact]
        void Can_deliver_same_tag_for_same_instance()
        {
            var obj = new object();
            var tag1 = obj.GetOrCreateTag();
            var tag2 = obj.GetOrCreateTag();

            tag1.Should().Be(tag2);
        }
    }
}

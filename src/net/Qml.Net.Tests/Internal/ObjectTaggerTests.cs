using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Qml.Net.Internal;
using Xunit;

namespace Qml.Net.Tests.Internal
{
    public class ObjectTaggerTests : BaseTests
    {
        public ObjectTaggerTests()
        {
        }

        [Fact]
        void Can_detect_id_overflow()
        {
            ObjectTagger tagger = new ObjectTagger(10);
            List<object> handledObjects = new List<object>();
            for (int i = 0; i < 9; i++)
            {
                var obj = new object();
                tagger.GetOrCreateTag(obj);
                handledObjects.Add(obj);
            }
            var lastObj = new object();
            Assert.Throws<Exception>(() => { tagger.GetOrCreateTag(lastObj); });
        }

        [Fact]
        void Can_handle_id_overflow_by_filling_spots()
        {
            ObjectTagger tagger = new ObjectTagger(10);
            List<object> handledObjects = new List<object>();
            for (int i = 0; i < 9; i++)
            {
                var obj = new object();
                tagger.GetOrCreateTag(obj);
                handledObjects.Add(obj);
            }

            // Ids are all used.
            handledObjects.Clear();
            GC.Collect(2, GCCollectionMode.Forced, true);
            Thread.Sleep(100);
            // The next one is the already prepared next id.
            var obj10 = new object();
            var tag10 = tagger.GetOrCreateTag(obj10);
            tag10.Should().Be(10ul);
            // The next after that will overflow.
            var obj1 = new object();
            var tag1 = tagger.GetOrCreateTag(obj1);
            tag1.Should().Be(1ul);
        }

        [Fact]
        void Can_deliver_same_tag_for_same_instance()
        {
            ObjectTagger tagger = new ObjectTagger();
            var obj = new object();
            var tag1 = tagger.GetOrCreateTag(obj);
            var tag2 = tagger.GetOrCreateTag(obj);

            tag1.Should().Be(tag2);
        }
    }
}

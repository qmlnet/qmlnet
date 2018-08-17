using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SerializationTests : BaseQmlTests<SerializationTests.SerializationTestsQml>
    {
        public class SerializationTestsQml
        {
            public virtual JsonObject GetJsonObject()
            {
                return null;
            }
            
            public virtual string Result { get; set; }
        }

        public class JsonObject
        {
            public string Prop1 { get; set; }
            
            public int Prop2 { get; set; }
        }
        
        [Fact]
        public void Can_serialize_object()
        {
            var jsonObject = new JsonObject();
            jsonObject.Prop1 = "test1";
            jsonObject.Prop2 = 3;
            var jsonObjectSerialized = Serializer.Current.Serialize(jsonObject);
            Mock.Setup(x => x.GetJsonObject()).Returns(jsonObject);
            Mock.SetupSet(x => x.Result = jsonObjectSerialized);
            
            RunQmlTest(
                "test",
                @"
                    var jsonObject = test.getJsonObject()
                    test.result = Net.serialize(jsonObject)
                ");
            
            Mock.Verify(x => x.GetJsonObject(), Times.Once);
            Mock.VerifySet(x => x.Result = jsonObjectSerialized, Times.Once);
        }
    }
}
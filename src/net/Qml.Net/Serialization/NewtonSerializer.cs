using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Qml.Net.Serialization
{
    public class NewtonSerializer : ISerializer
    {
        private JsonSerializerSettings _defaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _defaultSerializerSettings);
        }
    }
}
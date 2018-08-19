using Qml.Net.Serialization;

namespace Qml.Net
{
    public class Serializer
    {
        public static ISerializer Current = new NewtonSerializer();
    }
}
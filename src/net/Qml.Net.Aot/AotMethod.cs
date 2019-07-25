using System.Reflection;

namespace Qml.Net.Aot
{
    public class AotMethod
    {
        public AotMethod(AotClass decaringType, MethodInfo methodInfo)
        {
            DeclaringType = decaringType;
            MethodInfo = methodInfo;
        }
        
        public MethodInfo MethodInfo { get; }
        
        public AotClass DeclaringType { get; }
    }
}
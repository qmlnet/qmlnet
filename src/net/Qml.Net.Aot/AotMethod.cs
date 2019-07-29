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

        public string MethodName
        {
            get
            {
                var methodName = MethodInfo.Name;
                
                if (!char.IsLower(methodName[0]))
                {
                    methodName = char.ToLower(methodName[0]) + methodName.Substring(1);
                }

                return methodName;
            }
        }
        
        public MethodInfo MethodInfo { get; }
        
        public AotClass DeclaringType { get; }
    }
}
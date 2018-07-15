using System;

namespace Qt.NetCore.Sandbox
{
    public class Program
    {
        public class TestType
        {
            
        }
        
        static void Main()
        {
            Console.WriteLine("test");
            var typeInfo = NetTypeManager.GetTypeInfo<TestType>();
            Console.WriteLine(typeInfo.PrefVariantType);
        }
    }
}
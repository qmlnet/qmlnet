namespace Qml.Net.Aot
{
    public class AotSessionOptions
    {
        public AotSessionOptions()
        {
            Name = "Aot";
            NetNamespace = "Aot";
        }
        
        public string Name { get; set; }
        
        public string NetNamespace { get; set; }
    }
}
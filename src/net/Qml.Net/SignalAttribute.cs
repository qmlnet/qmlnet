using System;

namespace Qml.Net
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SignalAttribute : Attribute
    {
        public SignalAttribute(string name, params NetVariantType[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
        
        
        public string Name { get; }
        
        public NetVariantType[] Parameters { get; }
    }
}
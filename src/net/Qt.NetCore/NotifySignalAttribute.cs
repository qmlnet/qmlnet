using System;

namespace Qt.NetCore
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotifySignalAttribute : Attribute
    {
        public NotifySignalAttribute()
        {
            
        }
        
        public NotifySignalAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }
    }
}
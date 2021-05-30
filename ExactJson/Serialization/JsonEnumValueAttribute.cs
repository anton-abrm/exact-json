using System;

namespace ExactJson.Serialization
{
    public sealed class JsonEnumValueAttribute : Attribute
    {
        public JsonEnumValueAttribute() 
            : this(null) { }

        public JsonEnumValueAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
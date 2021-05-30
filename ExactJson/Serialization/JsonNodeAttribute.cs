using System;

namespace ExactJson.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class JsonNodeAttribute : Attribute
    {
        public JsonNodeAttribute() { }

        public JsonNodeAttribute(int position)
            : this(null, position) { }

        public JsonNodeAttribute(string name)
            : this(name, 0) { }

        public JsonNodeAttribute(string name, int position)
        {
            Position = position;
            Name = name;
        }

        public string Name { get; set; }
        public int Position { get; set; }
    }
}
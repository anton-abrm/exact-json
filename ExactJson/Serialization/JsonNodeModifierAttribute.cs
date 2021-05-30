using System;

namespace ExactJson.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class JsonNodeModifierAttribute : Attribute
    {
        public JsonNodeTarget ApplyTo { get; set; }
    }
}
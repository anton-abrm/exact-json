using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonNodeSerializationContext : IMetaContext
    {
        public bool? IsTuple { get; set; }
        public bool? IsOptional { get; set; }
        public bool? SerializeNullProperty { get; set; }
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public string TypePropertyName { get; set; }

        public JsonKeySerializationContext KeyContext { get; set; }
        public JsonItemSerializationContext ItemContext { get; set; }
        
        IMetaContext IMetaContext.ChildKey => KeyContext;
        IMetaContext IMetaContext.ChildItem => ItemContext;
    }
}
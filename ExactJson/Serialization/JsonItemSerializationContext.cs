using System;
using System.Diagnostics.CodeAnalysis;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonItemSerializationContext : IMetaContext
    {
        public bool? IsTuple { get; set; }
        public bool? IsOptional { get; set; }
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public string TypePropertyName { get; set; }

        [ExcludeFromCodeCoverage]
        bool? IMetaContext.SerializeNullProperty => null;
        
        [ExcludeFromCodeCoverage]
        IMetaContext IMetaContext.KeyContext => null;
        
        [ExcludeFromCodeCoverage]
        IMetaContext IMetaContext.ItemContext => null;
    }
}
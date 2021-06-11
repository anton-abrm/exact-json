using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonKeySerializationContext : IMetaContext
    {
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        
        bool? IMetaContext.IsTuple => null;
        bool? IMetaContext.IsOptional => null;
        bool? IMetaContext.SerializeNullProperty => null;
        string IMetaContext.TypePropertyName => null;
        
        IMetaContext IMetaContext.ChildKey => null;
        IMetaContext IMetaContext.ChildItem => null;
    }

}
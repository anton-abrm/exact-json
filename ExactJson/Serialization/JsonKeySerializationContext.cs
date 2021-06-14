using System;
using System.Diagnostics.CodeAnalysis;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonKeySerializationContext : IMetaContext
    {
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        
        [ExcludeFromCodeCoverage] 
        bool? IMetaContext.IsTuple => null;
        
        [ExcludeFromCodeCoverage] 
        bool? IMetaContext.IsOptional => null;
        
        [ExcludeFromCodeCoverage] 
        bool? IMetaContext.SerializeNullProperty => null;
        
        [ExcludeFromCodeCoverage] 
        string IMetaContext.TypePropertyName => null;
        
        [ExcludeFromCodeCoverage] 
        IMetaContext IMetaContext.ChildKey => null;
        
        [ExcludeFromCodeCoverage] 
        IMetaContext IMetaContext.ChildItem => null;
    }

}
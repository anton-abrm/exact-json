using System;

namespace ExactJson.Serialization.Meta
{
    internal interface IMetaContext
    {
        bool? IsTuple { get; }
        bool? IsOptional { get; }
        JsonConverter Converter { get; }
        string Format { get; }
        IFormatProvider FormatProvider { get; }
        string TypePropertyName { get; }
        bool? SerializeNullProperty { get; }

        IMetaContext ChildKey { get; }
        IMetaContext ChildItem { get; }
    }
}
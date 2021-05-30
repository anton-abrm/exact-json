using System;

namespace ExactJson.Serialization
{
    public struct JsonConverterContext
    {
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public Type TargetType { get; set; }
    }
}
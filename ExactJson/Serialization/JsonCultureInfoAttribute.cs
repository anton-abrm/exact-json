using System;
using System.Globalization;

namespace ExactJson.Serialization
{
    public sealed class JsonCultureInfoAttribute : JsonNodeModifierAttribute
    {
        public JsonCultureInfoAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public IFormatProvider CreateProvider()
        {
            return CultureInfo.GetCultureInfo(Name);
        }
    }
}
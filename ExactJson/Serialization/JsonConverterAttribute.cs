using System;

namespace ExactJson.Serialization
{
    public class JsonConverterAttribute : JsonNodeModifierAttribute
    {
        private readonly Type _converterType;

        public JsonConverterAttribute(Type converterType)
        {
            _converterType = converterType;
        }
        
        public virtual JsonConverter CreateConverter(Type targetType)
        {
            return (JsonConverter) Activator.CreateInstance(_converterType);
        }
    }
}
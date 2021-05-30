namespace ExactJson.Serialization
{
    public sealed class JsonSerializeNullAttribute : JsonNodeModifierAttribute
    {
        public JsonSerializeNullAttribute() 
            : this(true) { }

        public JsonSerializeNullAttribute(bool serializeNull)
        {
            SerializeNull = serializeNull;
        }
        
        public bool SerializeNull { get; }
    }
}
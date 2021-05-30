namespace ExactJson.Serialization
{
    public abstract class JsonConverter
    {
        public abstract void Write(JsonWriter output, object value, JsonConverterContext context);
        public abstract object Read(JsonReader input, JsonConverterContext context);
    }
}
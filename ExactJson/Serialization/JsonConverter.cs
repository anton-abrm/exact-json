namespace ExactJson.Serialization
{
    public abstract class JsonConverter
    {
        public abstract string GetString(object value, JsonConverterContext context);
        public abstract object GetValue(string s, JsonConverterContext context);
    }
}
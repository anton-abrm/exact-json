using System;

namespace ExactJson.Serialization
{
    public abstract class JsonStringConverter : JsonConverter
    {
        public abstract string GetString(object value, JsonConverterContext context);
        public abstract object GetValue(string s, JsonConverterContext context);
        
        public override void Write(JsonWriter output, object value, JsonConverterContext context)
        {
            if (output is null) {
                throw new ArgumentNullException(nameof(output));
            }
            
            output.WriteString(GetString(value, context));
        }

        public override object Read(JsonReader input, JsonConverterContext context)
        {
            if (input is null) {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.TokenType != JsonTokenType.String) {
                throw new JsonInvalidTypeException();
            }

            return GetValue(input.ReadString(), context);
        }
    }
}
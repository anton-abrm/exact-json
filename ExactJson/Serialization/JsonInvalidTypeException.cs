using System;

namespace ExactJson.Serialization
{
    public class JsonInvalidTypeException : JsonException
    {
        public JsonInvalidTypeException(string message = null, Exception innerException = null)
            : base(message ?? "Type is invalid.", innerException)
        { }
    }
}
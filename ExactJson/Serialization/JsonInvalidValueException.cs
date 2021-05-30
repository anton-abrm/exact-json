using System;

namespace ExactJson.Serialization
{
    public class JsonInvalidValueException : JsonException
    {
        public JsonInvalidValueException(string message = null, Exception innerException = null)
            : base(message ?? "Value is invalid.", innerException)
        { }
    }
}
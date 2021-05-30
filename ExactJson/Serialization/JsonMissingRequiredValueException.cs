using System;

namespace ExactJson.Serialization
{
    public class JsonMissingRequiredValueException : JsonException
    {
        internal JsonMissingRequiredValueException(string message = null, Exception innerException = null)
            : base(message ?? "Value is required.", innerException)
        { }
    }
}
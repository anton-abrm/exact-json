using System;

namespace ExactJson.Serialization
{
    public sealed class JsonValueOutOfRangeException : JsonInvalidValueException
    {
        internal JsonValueOutOfRangeException(string message = null, Exception innerException = null)
            : base(message ?? "Value is out of range.", innerException)
        { }
    }
}
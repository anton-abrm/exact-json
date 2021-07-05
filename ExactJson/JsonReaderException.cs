using System;

namespace ExactJson
{
    public abstract class JsonReaderException : JsonException
    {
        protected JsonReaderException(string message = null, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}
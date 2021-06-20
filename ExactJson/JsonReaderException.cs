using System;
using System.Runtime.Serialization;

namespace ExactJson
{
    public abstract class JsonReaderException : JsonException
    {
        protected JsonReaderException(string message = null, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}
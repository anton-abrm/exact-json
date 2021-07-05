using System;

namespace ExactJson
{
    public abstract class JsonException : Exception
    {
        protected JsonException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
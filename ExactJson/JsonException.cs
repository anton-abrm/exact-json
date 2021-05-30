using System;
using System.Runtime.Serialization;

namespace ExactJson
{
    [Serializable]
    public abstract class JsonException : Exception
    {
        protected JsonException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected JsonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
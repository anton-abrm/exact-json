using System;
using System.Runtime.Serialization;

namespace ExactJson
{
    [Serializable]
    public abstract class JsonReaderException : JsonException
    {
        protected JsonReaderException(string message = null, Exception innerException = null)
            : base(message, innerException)
        { }

        protected JsonReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
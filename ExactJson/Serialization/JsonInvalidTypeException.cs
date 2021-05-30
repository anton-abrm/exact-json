using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExactJson.Serialization
{
    [Serializable]
    public class JsonInvalidTypeException : JsonException
    {
        public JsonInvalidTypeException(string message = null, Exception innerException = null)
            : base(message ?? "Type is invalid.", innerException)
        { }
    }
}
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExactJson.Serialization
{
    public sealed class JsonSerializationException : JsonException
    {
        private static string FormatMessage(JsonPointer pointer, JsonException innerException)
            => innerException is IJsonLineInfo li 
                ? $"Serialization error occured. Pointer: '{pointer}' Position: ({li.LineNumber}:{li.LinePosition})" 
                : $"Serialization error occured. Pointer: '{pointer}'";
        
        internal JsonSerializationException(JsonPointer pointer, JsonException innerException)
            : base(FormatMessage(pointer, innerException), innerException)
        {
            if (pointer is null) {
                throw new ArgumentNullException(nameof(pointer));
            }

            if (innerException is null) {
                throw new ArgumentNullException(nameof(innerException));
            }

            Pointer = pointer.ToString();
        }

        public string Pointer { get; }
    }
}
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExactJson.Serialization
{
    [Serializable]
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

        private JsonSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Pointer = info.GetString(nameof(Pointer));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Pointer), Pointer);
        }

        public string Pointer { get; }
    }
}
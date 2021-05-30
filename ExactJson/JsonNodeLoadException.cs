using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExactJson
{
    [Serializable]
    public sealed class JsonNodeLoadException : JsonException
    {
        private static string FormatMessage(JsonPointer pointer, JsonException innerException)
            => innerException is IJsonLineInfo li 
                ? $"Unable to load node. Pointer: '{pointer}' Position: ({li.LineNumber}:{li.LinePosition})" 
                : $"Unable to load node. Pointer: '{pointer}'";

        internal JsonNodeLoadException(JsonPointer pointer, JsonException innerException)
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

        private JsonNodeLoadException(SerializationInfo info, StreamingContext context)
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
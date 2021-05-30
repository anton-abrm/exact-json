using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExactJson
{
    [Serializable]
    public sealed class JsonCharReaderException : JsonReaderException, IJsonLineInfo
    {
        internal JsonCharReaderException(
            int lineNumber,
            int linePosition,
            string message = null,
            Exception innerException = null)
            : base(FormatMessage(lineNumber, linePosition, message), innerException)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        private JsonCharReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            LineNumber = info.GetInt32(nameof(LineNumber));
            LinePosition = info.GetInt32(nameof(LinePosition));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(LineNumber), LineNumber);
            info.AddValue(nameof(LinePosition), LinePosition);
        }

        public int LineNumber { get; }
        public int LinePosition { get; }

        private static string FormatMessage(int lineNumber, int linePosition, string message = null)
        {
            return $"{message ?? "Unable to parse json."} Position: ({lineNumber}:{linePosition})";
        }
    }
}
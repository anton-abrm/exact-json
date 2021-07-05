using System;

namespace ExactJson
{
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

        public int LineNumber { get; }
        public int LinePosition { get; }

        private static string FormatMessage(int lineNumber, int linePosition, string message = null)
            => $"{message ?? "Unable to parse json."} Position: ({lineNumber}:{linePosition})";
    }
}
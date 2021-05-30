using System;

namespace ExactJson
{
    public sealed class JsonNodeDiff
    {
        public JsonNodeDiff(JsonPointer pointer, JsonNode self, JsonNode other)
        {
            Pointer = pointer ?? throw new ArgumentNullException(nameof(pointer));
            Self = self;
            Other = other;
        }

        public JsonPointer Pointer { get; }
        public JsonNode Self { get; }
        public JsonNode Other { get; }
    }
}
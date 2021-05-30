using System.Text;

namespace ExactJson
{
    public abstract partial class JsonPointer
    {
        private sealed class JsonRootPointer : JsonPointer
        {
            public static JsonRootPointer Value { get; } = new JsonRootPointer();

            private JsonRootPointer()
                : base(null)
            { }

            public override bool Equals(JsonPointer other)
            {
                return other is JsonRootPointer;
            }

            public override int GetHashCode()
            {
                return 17;
            }

            protected override void WriteTo(StringBuilder output)
            { }

            protected override JsonNode Match(JsonNode node)
            {
                return node;
            }
        }
    }
}
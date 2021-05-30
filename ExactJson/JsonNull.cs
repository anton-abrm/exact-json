using System;

namespace ExactJson
{
    public sealed class JsonNull : JsonValue, IEquatable<JsonNull>
    {
        private static readonly JsonNull Value = new JsonNull();

        private JsonNull()
        { }

        public override JsonNodeType NodeType
            => JsonNodeType.Null;

        public static JsonNull Create()
        {
            return Value;
        }

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteNull();
        }

        #region Equality

        public bool Equals(JsonNull other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonNull);
        }

        public override int GetHashCode()
        {
            return 17;
        }

        #endregion
    }
}
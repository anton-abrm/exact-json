using System;

namespace ExactJson
{
    public sealed class JsonBool : JsonValue, IEquatable<JsonBool>
    {
        private static readonly JsonBool True = new JsonBool(true);
        private static readonly JsonBool False = new JsonBool(false);

        public static JsonBool Create(bool value)
        {
            return value ? True : False;
        }

        public bool Value { get; }

        public override JsonNodeType NodeType => JsonNodeType.Bool;

        private JsonBool(bool value)
        {
            Value = value;
        }

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteBool(Value);
        }

        #region Equality

        public bool Equals(JsonBool other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonBool);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }

        #endregion
    }
}
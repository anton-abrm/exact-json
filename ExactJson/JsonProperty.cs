using System;

namespace ExactJson
{
    public readonly struct JsonProperty : IEquatable<JsonProperty>
    {
        public static readonly JsonProperty Empty = new JsonProperty();

        public JsonProperty(string name, JsonNode node)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = node ?? throw new ArgumentNullException(nameof(node));
        }

        public string Name { get; }
        public JsonNode Value { get; }

        public override bool Equals(object obj)
        {
            return obj is JsonProperty property && Equals(property);
        }

        public static bool operator ==(JsonProperty left, JsonProperty right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(JsonProperty left, JsonProperty right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = hash * 23 + (Name is not null ? Name.GetHashCode() : 0);
                hash = hash * 23 + (Value is not null ? Value.GetHashCode() : 0);
                return hash;
            }
        }

        public bool Equals(JsonProperty other)
        {
            return Equals(Name, other.Name)
                && Equals(Value, other.Value);
        }
    }
}
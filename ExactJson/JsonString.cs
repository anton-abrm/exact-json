// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public sealed class JsonString : JsonValue, IEquatable<JsonString>
    {
        private static readonly JsonString Empty = new JsonString(string.Empty);

        public static JsonString Create(string value)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return value != string.Empty
                ? new JsonString(value)
                : Empty;
        }

        private JsonString(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override JsonNodeType NodeType => JsonNodeType.String;

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteString(Value);
        }

        #region Equality

        public bool Equals(JsonString other)
        {
            if (other is null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Value == other.Value;
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonString);
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
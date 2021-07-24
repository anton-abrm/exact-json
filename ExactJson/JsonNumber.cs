// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public sealed class JsonNumber : JsonValue, IEquatable<JsonNumber>
    {
        private JsonNumber(JsonDecimal value, JsonNumberFormat format)
        {
            Value = value;
            Format = format;
        }

        #region Create

        public static JsonNumber Create(JsonDecimal value, JsonNumberFormat format)
        {
            return new JsonNumber(value, format);
        }

        public static JsonNumber Create(float value, string format = null)
        {
            return Create((JsonDecimal) value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(double value, string format = null)
        {
            return Create((JsonDecimal) value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(decimal value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public static JsonNumber Create(sbyte value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(byte value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public static JsonNumber Create(ushort value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(short value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public static JsonNumber Create(uint value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(int value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public static JsonNumber Create(ulong value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public static JsonNumber Create(long value, string format = null)
        {
            return Create(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        #endregion

        public JsonDecimal Value { get; }

        public JsonNumberFormat Format { get; }

        public override JsonNodeType NodeType => JsonNodeType.Number;

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteNumber(Value, Format);
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonNumber);
        }

        public bool Equals(JsonNumber other)
        {
            return other is not null && other.Value == Value;
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return Value.ToString(Format);
        }
    }
}
// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExactJson
{
    public abstract partial class JsonNode
    {
        internal JsonNode() { }

        public abstract JsonNodeType NodeType { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb)) {
                WriteTo(sw);
            }

            return sb.ToString();
        }

        public abstract bool DeepEquals(JsonNode other);

        public abstract JsonNode Clone();

        #region EvaluatePointer

        public JsonNode EvaluatePointer(string pointer)
        {
            return EvaluatePointer(JsonPointer.Parse(pointer));
        }

        public JsonNode EvaluatePointer(JsonPointer pointer)
        {
            if (pointer is null) {
                throw new ArgumentNullException(nameof(pointer));
            }

            return pointer.Evaluate(this);
        }

        #endregion

        #region Load

        private static JsonObject LoadObject(JsonReader input, Stack<PointerSection> stack)
        {
            input.ReadStartObject();

            var result = new JsonObject();

            while (input.TokenType != JsonTokenType.EndObject) {

                var property = input.ReadProperty();

                stack.Push(new PointerSection(property));
                result[property] = LoadNode(input, stack);
                stack.Pop();
            }

            input.ReadEndObject();

            return result;
        }

        private static JsonArray LoadArray(JsonReader input, Stack<PointerSection> stack)
        {
            input.ReadStartArray();

            var array = new JsonArray();

            while (input.TokenType != JsonTokenType.EndArray) {
                stack.Push(new PointerSection(array.Count));
                array.Add(LoadNode(input, stack));
                stack.Pop();
            }

            input.ReadEndArray();

            return array;
        }

        private static JsonNode LoadNode(JsonReader input, Stack<PointerSection> stack)
        {
            JsonNode result;

            switch (input.TokenType) {

                case JsonTokenType.StartObject:
                    result = LoadObject(input, stack);
                    break;

                case JsonTokenType.StartArray:
                    result = LoadArray(input, stack);
                    break;

                case JsonTokenType.Null:
                    input.ReadNull();
                    result = JsonNull.Create();
                    break;

                case JsonTokenType.Bool:
                    result = JsonBool.Create(input.ReadBool());
                    break;

                case JsonTokenType.String:
                    result = JsonString.Create(input.ReadString());
                    break;

                case JsonTokenType.Number:
                    result = JsonNumber.Create(input.ReadNumber(out var format), format);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected reader token type '{input.TokenType}'.");
            }

            return result;
        }

        public static JsonNode Load(JsonReader input)
        {
            if (input is null) {
                throw new ArgumentNullException(nameof(input));
            }

            var stack = new Stack<PointerSection>();

            try {
                if (input.MoveToToken() != JsonTokenType.None) {
                    return LoadNode(input, stack);
                }

                throw new EndOfStreamException();
            }
            catch (JsonException e) {
                var pointer = stack.Reverse()
                                   .Aggregate(JsonPointer.Root(), (p, s) => s.AttachTo(p));

                throw new JsonNodeLoadException(pointer, e);
            }
        }

        public static JsonNode Parse(string json)
        {
            using (var jr = new JsonStringReader(json)) {
                return Load(jr);
            }
        }

        #endregion

        #region Save

        public abstract void WriteTo(JsonWriter writer);

        public void WriteTo(TextWriter output)
        {
            if (output is null) {
                throw new ArgumentNullException(nameof(output));
            }

            using (var jw = new JsonTextWriter(output)) {
                WriteTo(jw);
            }
        }

        #endregion

        #region Diff

        private static void DiffArray(JsonArray self, JsonNode other, ICollection<JsonNodeDiff> diffs, JsonPointer pointer)
        {
            if (other is JsonArray otherArray) {
                int minCount = Math.Min(self.Count, otherArray.Count);

                var idx = 0;

                for (; idx < minCount; idx++) {
                    DiffNode(self[idx], otherArray[idx], diffs, pointer.Attach(idx));
                }

                int maxCount = Math.Max(self.Count, otherArray.Count);

                if (self.Count < otherArray.Count) {
                    for (; idx < maxCount; idx++) {
                        diffs.Add(new JsonNodeDiff(pointer.Attach(idx), null, otherArray[idx]));
                    }
                }
                else {
                    for (; idx < maxCount; idx++) {
                        diffs.Add(new JsonNodeDiff(pointer.Attach(idx), self[idx], null));
                    }
                }
            }
            else {
                diffs.Add(new JsonNodeDiff(pointer, self, other));
            }
        }

        private static void DiffObject(JsonObject self, JsonNode other, ICollection<JsonNodeDiff> diffs, JsonPointer pointer)
        {
            if (other is JsonObject otherObject) {
                foreach (var property in self) {
                    DiffNode(property.Value, otherObject[property.Name], diffs, pointer.Attach(property.Name));
                }

                foreach (var property in otherObject) {
                    if (!self.ContainsKey(property.Name)) {
                        diffs.Add(new JsonNodeDiff(pointer.Attach(property.Name), null, property.Value));
                    }
                }
            }
            else {
                diffs.Add(new JsonNodeDiff(pointer, self, other));
            }
        }

        private static void DiffValue(JsonValue self, JsonNode other, ICollection<JsonNodeDiff> diffs, JsonPointer pointer)
        {
            if (!self.Equals(other)) {
                diffs.Add(new JsonNodeDiff(pointer, self, other));
            }
        }
        
        private static void DiffNode(JsonNode self, JsonNode other, ICollection<JsonNodeDiff> diffs, JsonPointer pointer)
        {
            switch (self.NodeType) {
                
                case JsonNodeType.Object:
                    DiffObject((JsonObject) self, other, diffs, pointer);
                    break;

                case JsonNodeType.Array:
                    DiffArray((JsonArray) self, other, diffs, pointer);
                    break;

                case JsonNodeType.Null:
                case JsonNodeType.Number:
                case JsonNodeType.String:
                case JsonNodeType.Bool:
                    DiffValue((JsonValue) self, other, diffs, pointer);
                    break;
            }
        }

        public JsonNodeDiff[] Diff(JsonNode other)
        {
            var diffs = new List<JsonNodeDiff>();

            DiffNode(this, other, diffs, JsonPointer.Root());

            return diffs.ToArray();
        }

        #endregion

        #region Casting

        public static implicit operator JsonNode(string value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonString.Create(value);
        }

        public static implicit operator JsonNode(byte value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(byte? value)
        {
            if (value.HasValue) {
                return JsonNumber.Create(value.Value);
            }

            return JsonNull.Create();
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(sbyte value)
        {
            return JsonNumber.Create(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(sbyte? value)
        {
            if (value.HasValue) {
                return JsonNumber.Create(value.Value);
            }

            return JsonNull.Create();
        }

        public static implicit operator JsonNode(short value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(short? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(ushort value)
        {
            return JsonNumber.Create(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(ushort? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(int value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(int? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(uint value)
        {
            return JsonNumber.Create(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(uint? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(long value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(long? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(ulong value)
        {
            return JsonNumber.Create(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonNode(ulong? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(decimal value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(decimal? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(double value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(double? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(float value)
        {
            return JsonNumber.Create(value);
        }

        public static implicit operator JsonNode(float? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonNumber.Create(value.Value);
        }

        public static implicit operator JsonNode(bool value)
        {
            return JsonBool.Create(value);
        }

        public static implicit operator JsonNode(bool? value)
        {
            if (value is null) {
                return JsonNull.Create();
            }

            return JsonBool.Create(value.Value);
        }

        #endregion
    }
}
// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;

using ExactJson.Infra;

namespace ExactJson
{
    public abstract partial class JsonPointer : IEquatable<JsonPointer>
    {
        private JsonPointer(JsonPointer parent)
        {
            Parent = parent;
        }

        public JsonPointer Parent { get; }

        public JsonPointer Attach(string name)
        {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            return int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out int index)
                ? (JsonPointer) new JsonIndexPointer(this, index)
                : (JsonPointer) new JsonStringPointer(this, name);
        }

        public JsonPointer Attach(int index)
        {
            return new JsonIndexPointer(this, index);
        }

        public override string ToString()
        {
            var sb = StringBuilderCache.Acquire();

            WriteTo(sb);

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public static JsonPointer Root()
        {
            return JsonRootPointer.Value;
        }

        public static JsonPointer Parse(string pointer)
        {
            if (pointer is null) {
                throw new ArgumentNullException(nameof(pointer));
            }

            var result = Root();

            string[] sections = pointer.Split('/');

            if (sections[0] != string.Empty) {
                throw new FormatException();
            }

            for (var i = 1; i < sections.Length; i++) {
                string section = sections[i].Replace("~1", "/")
                                            .Replace("~0", "~");

                result = result.Attach(section);
            }

            return result;
        }

        protected abstract void WriteTo(StringBuilder output);

        protected abstract JsonNode Match(JsonNode node);

        #region Equality

        public abstract bool Equals(JsonPointer other);

        public override bool Equals(object other)
        {
            return Equals(other as JsonPointer);
        }

        public abstract override int GetHashCode();

        #endregion

        public JsonNode Evaluate(JsonNode node)
        {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            return Match(node);
        }
    }
}
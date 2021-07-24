// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;

namespace ExactJson
{
    public abstract partial class JsonPointer
    {
        private sealed class JsonIndexPointer : JsonPointer
        {
            private readonly int _index;

            public JsonIndexPointer(JsonPointer parent, int index)
                : base(parent)
            {
                _index = index;
            }

            public override bool Equals(JsonPointer other)
            {
                var otherArray = other as JsonIndexPointer;

                if (otherArray is null || _index != otherArray._index) {
                    return false;
                }

                return Parent.Equals(otherArray.Parent);
            }

            public override int GetHashCode()
            {
                unchecked {
                    var hash = 17;
                    hash = hash * 23 + Parent.GetHashCode();
                    hash = hash * 23 + _index.GetHashCode();
                    return hash;
                }
            }

            protected override JsonNode Match(JsonNode node)
            {
                switch (Parent.Match(node)) {
                    case JsonArray ja when _index < ja.Count:
                        return ja[_index];

                    case JsonObject jo:
                        return jo[_index.ToString(CultureInfo.InvariantCulture)];
                }

                return null;
            }

            protected override void WriteTo(StringBuilder output)
            {
                Parent.WriteTo(output);

                output.Append('/');
                output.Append(_index);
            }
        }
    }
}
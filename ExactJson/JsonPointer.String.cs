// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace ExactJson
{
    public abstract partial class JsonPointer
    {
        private sealed class JsonStringPointer : JsonPointer
        {
            private readonly string _name;

            public JsonStringPointer(JsonPointer parent, string name)
                : base(parent)
            {
                _name = name;
            }

            public override bool Equals(JsonPointer other)
            {
                var otherObject = other as JsonStringPointer;

                if (otherObject is null || _name != otherObject._name) {
                    return false;
                }

                return Parent.Equals(otherObject.Parent);
            }

            public override int GetHashCode()
            {
                unchecked {
                    var hash = 17;
                    hash = hash * 23 + Parent.GetHashCode();
                    hash = hash * 23 + _name.GetHashCode();
                    return hash;
                }
            }

            protected override JsonNode Match(JsonNode node)
            {
                return Parent.Match(node) is JsonObject jo ? jo[_name] : null;
            }

            protected override void WriteTo(StringBuilder output)
            {
                Parent.WriteTo(output);

                output.Append('/');

                foreach (char ch in _name) {
                    switch (ch) {
                        case '~':
                            output.Append("~0");
                            continue;

                        case '/':
                            output.Append("~1");
                            continue;
                    }

                    output.Append(ch);
                }
            }
        }
    }
}
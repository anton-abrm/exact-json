// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public abstract class JsonValue : JsonNode, IEquatable<JsonValue>
    {
        public sealed override JsonNode Clone()
        {
            return this;
        }

        public sealed override bool DeepEquals(JsonNode other)
        {
            return Equals(other as JsonValue);
        }

        public sealed override bool Equals(object other)
        {
            return Equals(other as JsonValue);
        }

        public abstract bool Equals(JsonValue other);

        public abstract override int GetHashCode();
    }
}
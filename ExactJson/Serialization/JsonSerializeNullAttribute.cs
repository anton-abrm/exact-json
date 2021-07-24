// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson.Serialization
{
    public sealed class JsonSerializeNullAttribute : JsonNodeModifierAttribute
    {
        public JsonSerializeNullAttribute() 
            : this(true) { }

        public JsonSerializeNullAttribute(bool serializeNull)
        {
            SerializeNull = serializeNull;
        }
        
        public bool SerializeNull { get; }
    }
}
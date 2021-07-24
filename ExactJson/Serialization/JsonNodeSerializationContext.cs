// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonNodeSerializationContext : IMetaContext
    {
        public bool? IsTuple { get; set; }
        public bool? IsOptional { get; set; }
        public bool? SerializeNullProperty { get; set; }
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public string TypePropertyName { get; set; }

        public JsonKeySerializationContext KeyContext { get; set; }
        public JsonItemSerializationContext ItemContext { get; set; }
        
        IMetaContext IMetaContext.KeyContext => KeyContext;
        IMetaContext IMetaContext.ItemContext => ItemContext;
    }
}
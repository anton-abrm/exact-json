// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public sealed class JsonTypePropertyNameAttribute : JsonNodeModifierAttribute
    {
        public JsonTypePropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string PropertyName { get; set; }
    }
}
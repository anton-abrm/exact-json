// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public sealed class JsonFormatAttribute : JsonNodeModifierAttribute
    {
        public JsonFormatAttribute(string format)
        {
            Format = format ?? throw new ArgumentNullException(nameof(format));
        }

        public string Format { get; }
    }
}
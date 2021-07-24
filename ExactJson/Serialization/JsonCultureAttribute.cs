// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace ExactJson.Serialization
{
    public sealed class JsonCultureAttribute : JsonNodeModifierAttribute
    {
        public JsonCultureAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        private string Name { get; }

        public IFormatProvider CreateProvider()
        {
            return CultureInfo.GetCultureInfo(Name);
        }
    }
}
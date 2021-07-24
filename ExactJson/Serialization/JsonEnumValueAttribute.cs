// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public sealed class JsonEnumValueAttribute : Attribute
    {
        public JsonEnumValueAttribute() 
            : this(null) { }

        public JsonEnumValueAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
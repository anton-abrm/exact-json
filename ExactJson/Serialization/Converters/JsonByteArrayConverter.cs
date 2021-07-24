// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonByteArrayConverter : JsonConverter
    {
        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return Convert.ToBase64String((byte[]) value);
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return Convert.FromBase64String(s);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}
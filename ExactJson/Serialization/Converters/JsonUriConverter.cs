// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonUriConverter : JsonConverter
    {
        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((Uri) value).ToString();
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return new Uri(s);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}
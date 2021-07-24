// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson.Serialization
{
    public abstract class JsonConverter
    {
        public abstract string GetString(object value, JsonConverterContext context);
        public abstract object GetValue(string s, JsonConverterContext context);
    }
}
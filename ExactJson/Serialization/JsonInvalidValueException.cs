// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public class JsonInvalidValueException : JsonException
    {
        public JsonInvalidValueException(string message = null, Exception innerException = null)
            : base(message ?? "Value is invalid.", innerException)
        { }
    }
}
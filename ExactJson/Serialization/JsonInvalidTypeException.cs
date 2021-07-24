// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public class JsonInvalidTypeException : JsonException
    {
        public JsonInvalidTypeException(string message = null, Exception innerException = null)
            : base(message ?? "Type is invalid.", innerException)
        { }
    }
}
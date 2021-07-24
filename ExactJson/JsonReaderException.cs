// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public abstract class JsonReaderException : JsonException
    {
        protected JsonReaderException(string message = null, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}
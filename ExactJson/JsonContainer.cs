// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson
{
    public abstract class JsonContainer : JsonNode
    {
        public JsonWriter CreateWriter()
            => new JsonNodeWriter(this);
    }
}
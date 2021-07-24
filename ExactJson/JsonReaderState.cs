// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public abstract class JsonReaderState
    {
        protected JsonReaderState(JsonReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }
        
        public JsonReader Reader { get; }

        public abstract void Restore();
    }
}
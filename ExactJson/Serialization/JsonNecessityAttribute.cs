// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson.Serialization
{
    public abstract class JsonNecessityAttribute : JsonNodeModifierAttribute
    {
        internal abstract bool IsOptional { get; }
    }
}
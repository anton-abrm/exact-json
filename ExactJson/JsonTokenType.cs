// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson
{
    public enum JsonTokenType
    {
        None,
        Property,
        StartObject,
        StartArray,
        EndObject,
        EndArray,
        Null,
        Bool,
        String,
        Number,
    }
}
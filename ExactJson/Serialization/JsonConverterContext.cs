﻿// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public struct JsonConverterContext
    {
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public Type TargetType { get; set; }
    }
}
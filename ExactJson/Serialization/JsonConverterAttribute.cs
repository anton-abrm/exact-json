// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Serialization
{
    public class JsonConverterAttribute : JsonNodeModifierAttribute
    {
        private readonly Type _converterType;
        
        public JsonConverterAttribute(Type converterType)
        {
            _converterType = converterType;
        }
        
        public virtual JsonConverter CreateConverter(Type targetType)
        {
            var instance = (JsonConverter) Activator.CreateInstance(_converterType);

            instance.SkipForNonStringValues = SkipForNonStringValues;
            
            return instance;
        }
        
        public bool SkipForNonStringValues { get; set; }
    }
}
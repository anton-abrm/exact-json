using System;

namespace ExactJson.Serialization
{
    public sealed class JsonOptionalAttribute : JsonNecessityAttribute
    {
        internal override bool IsOptional 
            => true;
    }
}
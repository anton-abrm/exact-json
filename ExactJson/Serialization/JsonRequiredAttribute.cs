namespace ExactJson.Serialization
{
    public sealed class JsonRequiredAttribute : JsonNecessityAttribute
    {
        internal override bool IsOptional 
            => false;
    }
}
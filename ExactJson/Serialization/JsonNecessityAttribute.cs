namespace ExactJson.Serialization
{
    public abstract class JsonNecessityAttribute : JsonNodeModifierAttribute
    {
        internal abstract bool IsOptional { get; }
    }
}
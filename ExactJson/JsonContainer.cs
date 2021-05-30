namespace ExactJson
{
    public abstract class JsonContainer : JsonNode
    {
        public JsonWriter CreateWriter()
            => new JsonNodeWriter(this);
    }
}
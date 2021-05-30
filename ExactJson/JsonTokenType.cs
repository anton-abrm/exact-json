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
namespace ExactJson
{
    public sealed partial class JsonNodeReader
    {
        private readonly struct Slot
        {
            public Slot(JsonNode value, bool isStart, bool isProperty, string propertyName, int index)
            {
                Value = value;
                IsStart = isStart;
                Index = index;
                IsProperty = isProperty;
                PropertyName = propertyName;
            }

            public bool IsStart { get; }
            public bool IsProperty { get; }
            public string PropertyName { get; }
            public int Index { get; }
            public JsonNode Value { get; }

            public Slot SetIsProperty(bool isProperty)
            {
                return new Slot(Value, IsStart, isProperty, PropertyName, Index);
            }

            public Slot SetIsStart(bool isStart)
            {
                return new Slot(Value, isStart, IsProperty, PropertyName, Index);
            }
        }
    }
}
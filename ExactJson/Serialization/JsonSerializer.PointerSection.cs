namespace ExactJson.Serialization
{
    public sealed partial class JsonSerializer
    {
        private readonly struct PointerSection
        {
            public PointerSection(string name)
            {
                _name = name;
                _index = 0;
            }

            public PointerSection(int index)
            {
                _name = null;
                _index = index;
            }

            private readonly string _name;
            private readonly int _index;

            public JsonPointer AttachTo(JsonPointer pointer)
            {
                return _name != null
                    ? pointer.Attach(_name)
                    : pointer.Attach(_index);
            }
        }
    }
}
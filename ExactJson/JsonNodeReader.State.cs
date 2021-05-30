namespace ExactJson
{
    public sealed partial class JsonNodeReader
    {
        private sealed class State : JsonReaderState
        {
            private readonly Slot[] _array;
            private readonly int _index;

            public State(JsonNodeReader reader) : base(reader)
            {
                _index = reader._index;
                _array = reader._stack.ToArray();
            }

            public override void Restore()
            {
                var reader = (JsonNodeReader) Reader;

                reader._index = _index;
                
                reader._stack.Clear();
                
                for (var i = _array.Length - 1; i >= 0; i--) {
                    reader._stack.Push(_array[i]);
                }
            }
        }
    }
}
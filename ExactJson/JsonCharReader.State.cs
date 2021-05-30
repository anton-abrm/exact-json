namespace ExactJson
{
    public abstract partial class JsonCharReader
    {
        private sealed class State : JsonReaderState
        {
            private readonly JsonNodeType[] _array;
            private readonly long _position;
            private readonly int _lineNumber;
            private readonly int _linePosition;
            private readonly int _prevLineNumber;
            private readonly int _prevLinePosition;
            private readonly JsonTokenType _tokenType;
            private readonly bool _valueBool;
            private readonly string _valueString;
            private readonly JsonDecimal _valueNumber;
            private readonly JsonNumberFormat _numberFormat;

            public State(JsonCharReader reader) : base(reader)
            {
                _position = reader.Position;
                
                _lineNumber = reader._lineNumber;
                _linePosition = reader._linePosition;
                _prevLineNumber = reader._prevLineNumber;
                _prevLinePosition = reader._prevLinePosition;

                _array = reader._stack.ToArray();

                _tokenType = reader._tokenType;

                switch (_tokenType) {
                    
                    case JsonTokenType.Property:
                    case JsonTokenType.String:
                        _valueString = reader.ValueAsString;
                        break;
                   
                    case JsonTokenType.Bool:
                        _valueBool = reader.ValueAsBool;
                        break;
                    
                    case JsonTokenType.Number:
                        _numberFormat = reader.NumberFormat;
                        _valueNumber = reader.ValueAsNumber;
                        break;
                }
            }

            public override void Restore()
            {
                var reader = (JsonCharReader) Reader;
                
                reader.Position = _position;

                reader._stack.Clear();

                for (var i = _array.Length - 1; i >= 0; i--) {
                    reader._stack.Push(_array[i]);
                }

                reader._lineNumber = _lineNumber;
                reader._linePosition = _linePosition;
                reader._prevLineNumber = _prevLineNumber;
                reader._prevLinePosition = _prevLinePosition;

                reader._tokenType = _tokenType;

                switch (_tokenType) {
                    
                    case JsonTokenType.Property:
                    case JsonTokenType.String:
                        reader._valueString = _valueString;
                        break;

                    case JsonTokenType.Bool:
                        reader._valueBool = _valueBool;
                        break;

                    case JsonTokenType.Number:
                        reader._numberFormat = _numberFormat;
                        reader._valueNumber = _valueNumber;
                        break;
                }
            }
        }
    }
}
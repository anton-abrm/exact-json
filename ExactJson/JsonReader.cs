using System;

namespace ExactJson
{
    public abstract class JsonReader : IDisposable
    {
        public abstract JsonTokenType TokenType { get; }

        public abstract bool ValueAsBool { get; }
        public abstract string ValueAsString { get; }
        public abstract JsonDecimal ValueAsNumber { get; }
        public abstract JsonNumberFormat NumberFormat { get; }

        public abstract bool Read();

        public virtual void Skip()
        {
            if (TokenType != JsonTokenType.None) {
                SkipNode();
            }
            else {
                Read();
                while (TokenType != JsonTokenType.None) {
                    SkipNode();
                }
            }
            
            void SkipNode()
            {
                switch (TokenType) {
                    case JsonTokenType.Bool:
                    case JsonTokenType.Null:
                    case JsonTokenType.Number:
                    case JsonTokenType.String:
                        SkipToken();
                        return;

                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        for (var d = SkipToken(); d > 0; d += SkipToken()) { }
                        return;
                
                    case JsonTokenType.Property:
                    case JsonTokenType.EndObject:
                    case JsonTokenType.EndArray:
                        throw new InvalidOperationException("Invalid reader state.");
                }
            }
            
            int SkipToken()
            {
                var delta = 0;
                
                switch (TokenType) {

                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        delta = 1;
                        break;

                    case JsonTokenType.EndObject:
                    case JsonTokenType.EndArray:
                        delta = -1;
                        break;
                }
                
                ReadToken();

                return delta;
            }
        }

        public virtual void CopyTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            if (TokenType != JsonTokenType.None) {
                CopyNode();
            }
            else {
                Read();
                while (TokenType != JsonTokenType.None) {
                    CopyNode();
                }
            }

            void CopyNode()
            {
                switch (TokenType) 
                {
                    case JsonTokenType.Bool:
                    case JsonTokenType.Null:
                    case JsonTokenType.Number:
                    case JsonTokenType.String:
                        CopyToken();
                        break;
                    
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        for (var d = CopyToken(); d > 0; d += CopyToken()) { }
                        break;

                    default:
                        throw new InvalidOperationException("Invalid reader state.");
                }
            }
            
            int CopyToken()
            {
                var delta = 0;
                
                switch (TokenType) {
                    
                    case JsonTokenType.Property:
                        writer.WriteProperty(ValueAsString);
                        break;

                    case JsonTokenType.StartObject:
                        writer.WriteStartObject();
                        delta = 1;
                        break;

                    case JsonTokenType.StartArray:
                        writer.WriteStartArray();
                        delta = 1;
                        break;

                    case JsonTokenType.EndObject:
                        writer.WriteEndObject();
                        delta = -1;
                        break;

                    case JsonTokenType.EndArray:
                        writer.WriteEndArray();
                        delta = -1;
                        break;

                    case JsonTokenType.Null:
                        writer.WriteNull();
                        break;

                    case JsonTokenType.Bool:
                        writer.WriteBool(ValueAsBool);
                        break;

                    case JsonTokenType.String:
                        writer.WriteString(ValueAsString);
                        break;

                    case JsonTokenType.Number:
                        writer.WriteNumber(ValueAsNumber, NumberFormat);
                        break;
                }

                ReadToken();

                return delta;
            }
        }

        protected virtual void Dispose(bool disposing) { }

        public object Value
        {
            get
            {
                switch (TokenType) {
                    
                    case JsonTokenType.Property:
                    case JsonTokenType.String:
                        return ValueAsString;

                    case JsonTokenType.Bool:
                        return ValueAsBool;

                    case JsonTokenType.Number:
                        return ValueAsNumber;
                }

                return null;
            }
        }
        
        public abstract bool CanSaveState { get; }

        public abstract JsonReaderState SaveState();

        #region Read Helpers
        
        public virtual bool MoveToProperty(string propertyName)
        {
            if (propertyName is null) {
                throw new ArgumentNullException(nameof(propertyName));
            }

            switch (TokenType) {

                case JsonTokenType.Property:
                    break;
                
                case JsonTokenType.StartObject:
                    ReadStartObject();
                    break;
                
                default:
                    throw new InvalidOperationException($"Unexpected token: {TokenType}.");
            }

            while (TokenType != JsonTokenType.EndObject) {

                if (ValueAsString == propertyName) {
                    return true;
                }

                ReadProperty();
                Skip();
            }

            return false;
        }

        
        private void EnsureToken(JsonTokenType tokenType)
        {
            if (TokenType != tokenType) {
                throw new InvalidOperationException($"Expected '{tokenType}' but got '{TokenType}'.");
            }
        }

        public string ReadString()
        {
            EnsureToken(JsonTokenType.String);
            string result = ValueAsString;
            Read();
            return result;
        }

        public bool ReadBool()
        {
            EnsureToken(JsonTokenType.Bool);
            bool result = ValueAsBool;
            Read();
            return result;
        }

        public string ReadProperty()
        {
            EnsureToken(JsonTokenType.Property);
            string result = ValueAsString;
            Read();
            return result;
        }

        public JsonDecimal ReadNumber()
            => ReadNumber(out _);
        
        public JsonDecimal ReadNumber(out JsonNumberFormat format)
        {
            EnsureToken(JsonTokenType.Number);
            format = NumberFormat;
            var result = ValueAsNumber;
            Read();
            return result;
        }
        
        public void ReadNull()
        {
            EnsureToken(JsonTokenType.Null);
            Read();
        }

        public void ReadStartObject()
        {
            EnsureToken(JsonTokenType.StartObject);
            Read();
        }

        public void ReadEndObject()
        {
            EnsureToken(JsonTokenType.EndObject);
            Read();
        }

        public void ReadStartArray()
        {
            EnsureToken(JsonTokenType.StartArray);
            Read();
        }

        public void ReadEndArray()
        {
            EnsureToken(JsonTokenType.EndArray);
            Read();
        }

        public void ReadToken()
        {
            if (TokenType == JsonTokenType.None) {
                throw new InvalidOperationException($"Unexpected '{TokenType}'.");
            }

            Read();
        }

        public JsonTokenType MoveToToken()
        {
            if (TokenType == JsonTokenType.None) {
                Read();
            }

            return TokenType;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
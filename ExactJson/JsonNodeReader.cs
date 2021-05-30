using System;
using System.Collections.Generic;

namespace ExactJson
{
    public sealed partial class JsonNodeReader : JsonReader
    {
        private readonly IList<JsonNode> _nodes;
        private readonly Stack<Slot> _stack = new Stack<Slot>();

        private int _index;

        public JsonNodeReader(params JsonNode[] nodes)
        {
            if (nodes is null) {
                throw new ArgumentNullException(nameof(nodes));
            }
            
            for (var i = 0; i < nodes.Length; i++) {
                if (nodes[i] is null) {
                    throw new ArgumentException($"Array contains null at index {i}.", nameof(nodes));
                }
            }
            
            _nodes = nodes;
        }

        public override JsonTokenType TokenType
        {
            get
            {
                if (_stack.Count == 0) {
                    return JsonTokenType.None;
                }

                var slot = _stack.Peek();
                if (slot.IsProperty) {
                    return JsonTokenType.Property;
                }

                return slot.Value.NodeType switch {
                    JsonNodeType.Bool => JsonTokenType.Bool,
                    JsonNodeType.String => JsonTokenType.String,
                    JsonNodeType.Number => JsonTokenType.Number,
                    JsonNodeType.Array => slot.IsStart 
                        ? JsonTokenType.StartArray 
                        : JsonTokenType.EndArray,
                    JsonNodeType.Object => slot.IsStart 
                        ? JsonTokenType.StartObject 
                        : JsonTokenType.EndObject,
                    _ => JsonTokenType.Null
                };
            }
        }

        public override bool ValueAsBool
        {
            get
            {
                if (TokenType != JsonTokenType.Bool) {
                    throw new InvalidOperationException();
                }

                return ((JsonBool) _stack.Peek().Value).Value;
            }
        }

        public override string ValueAsString
        {
            get
            {
                switch (TokenType) {
                    
                    case JsonTokenType.Property:
                        return _stack.Peek().PropertyName;

                    case JsonTokenType.String:
                        return ((JsonString) _stack.Peek().Value).Value;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public override JsonDecimal ValueAsNumber
        {
            get
            {
                if (TokenType != JsonTokenType.Number) {
                    throw new InvalidOperationException();
                }

                return ((JsonNumber) _stack.Peek().Value).Value;
            }
        }

        public override JsonNumberFormat NumberFormat
        {
            get
            {
                if (TokenType != JsonTokenType.Number) {
                    throw new InvalidOperationException();
                }

                return ((JsonNumber) _stack.Peek().Value).Format;
            }
        }

        public override bool Read()
        {
            if (_index >= _nodes.Count) {
                return false;
            }

            if (_stack.Count == 0) {
                _stack.Push(new Slot(_nodes[_index], true, false, null, -1));
                return true;
            }

            var slot = _stack.Pop();
            if (slot.IsProperty) {
                _stack.Push(slot.SetIsProperty(false));
                return true;
            }

            switch (slot.Value) {
                
                case JsonArray arr:
                {
                    if (slot.IsStart) {
                        
                        _stack.Push(slot.SetIsStart(false));

                        if (arr.Count != 0) {
                            _stack.Push(new Slot(arr[0], true, false, null, 0));
                        }

                        return true;
                    }

                    break;
                }

                case JsonObject obj:
                {
                    if (slot.IsStart) {
                        
                        _stack.Push(slot.SetIsStart(false));

                        var property = obj.First;
                        if (property.HasValue) {
                            _stack.Push(new Slot(property.Value.Value, true, true, property.Value.Name, -1));
                        }

                        return true;
                    }

                    break;
                }
            }

            if (_stack.Count == 0) {
                _index++;
                
                if (_index < _nodes.Count) {
                    _stack.Push(new Slot(_nodes[_index], true, false, null, -1));
                    return true;
                }

                return false;
            }

            var parentSlot = _stack.Peek();

            switch (parentSlot.Value) {
                
                case JsonArray parentArr:
                {
                    int nextIndex = slot.Index + 1;
                    if (nextIndex < parentArr.Count) {
                        _stack.Push(new Slot(parentArr[nextIndex], true, false, null, nextIndex));
                    }
                    
                    break;
                }

                case JsonObject parentObj:
                {
                    var nextProperty = parentObj.Next(slot.PropertyName);
                    if (nextProperty != null) {
                        _stack.Push(new Slot(nextProperty.Value.Value, true, true, nextProperty.Value.Name, -1));
                    }

                    break;
                }
            }
            
            return true;
        }
        
        public override bool CanSaveState 
            => true;

        public override JsonReaderState SaveState() 
            => new State(this);

        public override bool MoveToProperty(string propertyName)
        {
            if (propertyName is null) {
                throw new ArgumentNullException(nameof(propertyName));
            }
            
            switch (TokenType) {

                case JsonTokenType.Property:
                    _stack.Pop();
                    break;

                case JsonTokenType.StartObject:
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var jsonObject = (JsonObject) _stack.Peek().Value;
            
            _stack.Pop();
            _stack.Push(new Slot(jsonObject, false, false, null, -1));
            
            var node = jsonObject[propertyName];
            if (node is null) {
                return false;
            }
            
            _stack.Push(new Slot(node, true, true, propertyName, -1));
            
            return true;
        }
    }
}
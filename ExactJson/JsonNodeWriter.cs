using System;
using System.Collections.Generic;

namespace ExactJson
{
    public class JsonNodeWriter : JsonWriter
    {
        private readonly struct Slot
        {
            public Slot(string name, JsonContainer node)
            {
                Name = name;
                Node = node;
            }

            public string Name { get; }
            public JsonContainer Node { get; }
        }

        private readonly Stack<Slot> _stack = new Stack<Slot>();
        private readonly List<JsonNode> _nodes = new List<JsonNode>();

        public JsonNodeWriter()
        { }

        public JsonNodeWriter(JsonContainer container)
        {
            if (container is null) {
                throw new ArgumentNullException(nameof(container));
            }

            _stack.Push(new Slot(null, container));
        }
        
        public override void WriteProperty(string name)
        {
            if (_stack.Count == 0 || 
                _stack.Peek().Node.NodeType != JsonNodeType.Object || 
                _stack.Peek().Name is not null) {
                throw new InvalidOperationException(
                    $"Token {JsonTokenType.Property} in the current state would result in an invalid JSON document.");
            }

            var slot = _stack.Pop();
            
            _stack.Push(new Slot(name, slot.Node));
        }
        
        public override void WriteNull()
        {
            WriteStartContainerOrValue(JsonNull.Create());
        }

        public override void WriteBool(bool value)
        {
            WriteStartContainerOrValue(JsonBool.Create(value));
        }

        public override void WriteNumber(JsonDecimal value, JsonNumberFormat format)
        {
            WriteStartContainerOrValue(JsonNumber.Create(value, format));
        }

        public override void WriteString(string value)
        {
            WriteStartContainerOrValue(JsonString.Create(value));
        }

        public override void WriteStartObject()
        {
            WriteStartContainerOrValue(new JsonObject());
        }
        
        public override void WriteEndObject()
        {
            if (_stack.Count == 0 || 
                _stack.Peek().Node.NodeType != JsonNodeType.Object || 
                _stack.Peek().Name is not null) {
                throw new InvalidOperationException(
                    $"Token {JsonTokenType.EndObject} in the current state would result in an invalid JSON document.");
            }

            var slot = _stack.Pop();

            if (_stack.Count == 0) {
                _nodes.Add(slot.Node);
            }
        }
        
        public override void WriteStartArray()
        {
            WriteStartContainerOrValue(new JsonArray());
        }
        
        public override void WriteEndArray()
        {
            if (_stack.Count == 0 || 
                _stack.Peek().Node.NodeType != JsonNodeType.Array) {
                throw new InvalidOperationException(
                    $"Token {JsonTokenType.EndArray} in the current state would result in an invalid JSON document.");
            }

            var slot = _stack.Pop();

            if (_stack.Count == 0) {
                _nodes.Add(slot.Node);
            }
        }
        
        private void WriteStartContainerOrValue(JsonNode node)
        {
            if (_stack.Count == 0) {

                if (node is JsonContainer container) {
                    _stack.Push(new Slot(null, container));
                }
                else {
                    _nodes.Add(node);
                }
            }
            else {
                
                var parent = _stack.Peek();

                switch (parent.Node) {

                    case JsonObject jsonObject:
                    {
                        var name = parent.Name;
                        if (name is null) {
                            throw new InvalidOperationException(
                                $"Node {node.NodeType} in the current state would result in an invalid JSON document.");
                        }
                    
                        jsonObject[name] = node;

                        _stack.Pop();
                        _stack.Push(new Slot(null, jsonObject));
                    
                        break;
                    }
                
                    case JsonArray jsonArray:
                    {
                        jsonArray.Add(node);
                        break;
                    }
                }
                
                if (node is JsonContainer container) {
                    _stack.Push(new Slot(null, container));
                }
            }
        }
        
        private void CloseOpenContainers()
        {
            while (_stack.Count != 0) {
                
                switch (_stack.Peek().Node.NodeType) {
                    
                    case JsonNodeType.Array:
                        WriteEndArray();
                        break;

                    case JsonNodeType.Object:
                        WriteEndObject();
                        break;
                }
            }
        }

        public JsonNode[] GetNodes()
        {
            CloseOpenContainers();

            return _nodes.ToArray();
        }

        public JsonNode GetNode()
        {
            CloseOpenContainers();
            
            if (_nodes.Count == 0) {
                throw new InvalidOperationException("Sequence is empty.");
            }
            
            if (_nodes.Count > 1) {
                throw new InvalidOperationException("Sequence contains more than one element.");
            }

            return _nodes[0];
        }
    }
}
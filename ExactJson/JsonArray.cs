using System;
using System.Collections;
using System.Collections.Generic;

namespace ExactJson
{
    public sealed class JsonArray : JsonContainer, IList<JsonNode>
    {
        private readonly List<JsonNode> _items = new List<JsonNode>();

        public JsonArray()
        { }

        private JsonArray(JsonArray other)
        {
            foreach (var item in other._items) {
                _items.Add(item.Clone());
            }
        }

        public JsonNode this[int index]
        {
            get => _items[index];
            set => _items[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int Count => _items.Count;

        bool ICollection<JsonNode>.IsReadOnly => false;

        public override JsonNodeType NodeType => JsonNodeType.Array;

        public void Add(JsonNode node)
        {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            _items.Add(node);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(JsonNode node)
        {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            return _items.Contains(node);
        }

        public void CopyTo(JsonNode[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<JsonNode> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(JsonNode item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, JsonNode item)
        {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Insert(index, item);
        }

        public bool Remove(JsonNode node)
        {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            return _items.Remove(node);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartArray();

            foreach (var item in _items) {
                item.WriteTo(writer);
            }

            writer.WriteEndArray();
        }

        public override bool DeepEquals(JsonNode other)
        {
            var otherArray = other as JsonArray;

            if (otherArray is null) {
                return false;
            }

            if (Count != otherArray.Count) {
                return false;
            }

            for (var i = 0; i < Count; i++) {
                if (!this[i].DeepEquals(otherArray[i])) {
                    return false;
                }
            }

            return true;
        }

        public override JsonNode Clone()
        {
            return new JsonArray(this);
        }
    }
}
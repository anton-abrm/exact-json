using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson
{
    public sealed class JsonObject : JsonContainer,
                                     ICollection<JsonProperty>,
                                     IDictionary<string, JsonNode>,
                                     ICollection<string>,
                                     ICollection<JsonNode>
    {
        private readonly struct Slot
        {
            public Slot(string key, JsonNode value, int prev, int next)
            {
                Key = key;
                Value = value;
                Prev = prev;
                Next = next;
            }

            public readonly string Key;
            public readonly JsonNode Value;
            public readonly int Next;
            public readonly int Prev;

            public Slot SetValue(JsonNode value)
                => new Slot(Key, value, Prev, Next);

            public Slot SetNext(int next)
                => new Slot(Key, Value, Prev, next);

            public Slot SetPrev(int prev)
                => new Slot(Key, Value, prev, Next);

            public Slot Clone()
                => new Slot(Key, Value.Clone(), Prev, Next);

            public JsonProperty ToProperty()
                => new JsonProperty(Key, Value);

            public KeyValuePair<string, JsonNode> ToKeyValuePair()
                => new KeyValuePair<string, JsonNode>(Key, Value);
        }

        private const int None = -1;

        private readonly Dictionary<string, int> _dict;
        private readonly List<Slot> _list;

        private int _first = None;
        private int _last = None;


        public JsonObject()
        {
            _dict = new Dictionary<string, int>();
            _list = new List<Slot>();
        }

        private JsonObject(JsonObject other)
        {
            _dict = new Dictionary<string, int>(other._dict);
            _list = new List<Slot>(other._list.Capacity);

            foreach (var slot in other._list) {
                _list.Add(slot.Clone());
            }

            _first = other._first;
            _last = other._last;
        }

        #region Private

        private void SetImpl(int index, JsonNode value)
        {
            _list[index] = _list[index].SetValue(value);
        }

        private void AddImpl(string key, JsonNode value)
        {
            var toAdd = _list.Count;
            
            _list.Add(new Slot(key, value, _last, None));

            if (toAdd == 0) {
                _first = toAdd;
            }
            else {
                _list[_last] = _list[_last].SetNext(toAdd);
            }

            _last = toAdd;
            
            _dict.Add(key, toAdd);
        }

        private bool DelImpl(string key)
        {
            if (!_dict.TryGetValue(key, out var toDelete)) {
                return false;
            }

            var prev = _list[toDelete].Prev;
            var next = _list[toDelete].Next;

            if (prev == None) {
                _first = next;
            }
            else {
                _list[prev] = _list[prev].SetNext(next);
            }

            if (next == None) {
                _last = prev;
            }
            else {
                _list[next] = _list[next].SetPrev(prev);
            }

            _dict.Remove(key);

            // ------------
            //  Compact
            // ------------

            var toMove = _list.Count - 1;
            if (toMove != toDelete) {

                _list[toDelete] = _list[toMove];
              
                var toMovePrev = _list[toMove].Prev;
                var toMoveNext = _list[toMove].Next;

                if (toMovePrev == None) {
                    _first = toDelete;
                }
                else {
                    _list[toMovePrev] = _list[toMovePrev].SetNext(toDelete);
                }

                if (toMoveNext == None) {
                    _last = toDelete;
                }
                else {
                    _list[toMoveNext] = _list[toMoveNext].SetPrev(toDelete);
                }
                
                _dict[_list[toMove].Key] = toDelete;
            }

            _list.RemoveAt(toMove);

            return true;
        }

        private bool TryGetSlotImpl(string key, out Slot value)
        {
            if (!_dict.TryGetValue(key, out var index)) {
                value = default;
                return false;
            }

            value = _list[index];
            return true;
        }

        #endregion

        public override JsonNodeType NodeType => JsonNodeType.Object;

        public override void WriteTo(JsonWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var property in this) {
                writer.WriteProperty(property.Name);
                property.Value.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        public override bool DeepEquals(JsonNode other)
        {
            var otherObject = other as JsonObject;
            if (otherObject is null) {
                return false;
            }

            if (Count != otherObject.Count) {
                return false;
            }

            foreach (var property in this) {

                var otherValue = otherObject[property.Name];
                if (otherValue is null) {
                    return false;
                }

                if (!property.Value.DeepEquals(otherValue)) {
                    return false;
                }
            }

            return true;
        }

        public override JsonNode Clone()
        {
            return new JsonObject(this);
        }

        public int Count => _list.Count;

        public JsonProperty? First
            => _first != None ? _list[_first].ToProperty() : null;

        public JsonProperty? Last
            => _last != None ? _list[_last].ToProperty() : null;

        public JsonProperty? Next(string propertyName)
        {
            if (propertyName is null) {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!_dict.TryGetValue(propertyName, out var index)) {
                throw new InvalidOperationException($"Property with name '{propertyName}' not found.");
            }

            var next = _list[index].Next;
            if (next != None) {
                return _list[next].ToProperty();
            }

            return null;
        }

        public JsonProperty? Previous(string propertyName)
        {
            if (propertyName is null) {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!_dict.TryGetValue(propertyName, out var index)) {
                throw new InvalidOperationException($"Property with name '{propertyName}' not found.");
            }

            var prev = _list[index].Prev;
            if (prev != None) {
                return _list[prev].ToProperty();
            }

            return null;
        }

        public JsonNode this[string name]
        {
            get
            {
                if (name is null) {
                    throw new ArgumentNullException(nameof(name));
                }

                if (TryGetSlotImpl(name, out var slot)) {
                    return slot.Value;
                }

                return null;
            }
            set
            {
                if (name is null) {
                    throw new ArgumentNullException(nameof(name));
                }

                if (value is null) {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_dict.TryGetValue(name, out var index)) {
                    SetImpl(index, value);
                }
                else {
                    AddImpl(name, value);
                }
            }
        }

        public void Add(string name, JsonNode node)
        {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (_dict.ContainsKey(name)) {
                throw new InvalidOperationException("Object already contains item.");
            }

            AddImpl(name, node);
        }

        public bool ContainsKey(string name)
        {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            return _dict.ContainsKey(name);
        }

        public void Clear()
        {
            _list.Clear();
            _dict.Clear();
            _first = None;
            _last = None;
        }

        public bool Remove(string name)
        {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            return DelImpl(name);
        }

        public IEnumerator<JsonProperty> GetEnumerator()
        {
            for (var index = _first; index != None; index = _list[index].Next) {
                yield return _list[index].ToProperty();
            }
        }

        #region ICollection<JsonProperty>

        void ICollection<JsonProperty>.Add(JsonProperty property)
        {
            if (property == JsonProperty.Empty) {
                throw new ArgumentException("Property can not be empty.", nameof(property));
            }

            if (_dict.ContainsKey(property.Name)) {
                throw new InvalidOperationException("Object already contains item.");
            }

            AddImpl(property.Name, property.Value);
        }

        bool ICollection<JsonProperty>.IsReadOnly => false;

        bool ICollection<JsonProperty>.Contains(JsonProperty item)
        {
            if (item == JsonProperty.Empty) {
                throw new ArgumentException("Item can not be empty.", nameof(item));
            }

            return TryGetSlotImpl(item.Name, out var slot) && slot.ToProperty().Equals(item);
        }

        void ICollection<JsonProperty>.CopyTo(JsonProperty[] array, int arrayIndex)
            => CollectionUtil.CopyTo(this, array, arrayIndex);

        bool ICollection<JsonProperty>.Remove(JsonProperty item)
        {
            if (item == JsonProperty.Empty) {
                throw new ArgumentException("Item can not be empty.", nameof(item));
            }

            if (!TryGetSlotImpl(item.Name, out var slot)) {
                return false;
            }

            if (!slot.ToProperty().Equals(item)) {
                return false;
            }

            return DelImpl(item.Name);
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

        #region IDictionary<string, JsonNode>

        ICollection<string> IDictionary<string, JsonNode>.Keys
            => this;

        ICollection<JsonNode> IDictionary<string, JsonNode>.Values
            => this;

        bool IDictionary<string, JsonNode>.TryGetValue(string key, out JsonNode value)
        {
            if (key is null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (TryGetSlotImpl(key, out var slot)) {
                value = slot.Value;
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region ICollection<KeyValuePair<string, JsonNode>>

        bool ICollection<KeyValuePair<string, JsonNode>>.IsReadOnly
            => false;

        void ICollection<KeyValuePair<string, JsonNode>>.Add(KeyValuePair<string, JsonNode> item)
        {
            ((ICollection<JsonProperty>) this).Add(new JsonProperty(item.Key, item.Value));
        }

        bool ICollection<KeyValuePair<string, JsonNode>>.Contains(KeyValuePair<string, JsonNode> item)
        {
            return ((ICollection<JsonProperty>) this).Contains(new JsonProperty(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, JsonNode>>.CopyTo(KeyValuePair<string, JsonNode>[] array, int arrayIndex)
            => CollectionUtil.CopyTo(this, array, arrayIndex);

        bool ICollection<KeyValuePair<string, JsonNode>>.Remove(KeyValuePair<string, JsonNode> item)
        {
            return ((ICollection<JsonProperty>) this).Remove(new JsonProperty(item.Key, item.Value));
        }

        #endregion

        #region IEnumerable<KeyValuePair<string, JsonNode>>

        IEnumerator<KeyValuePair<string, JsonNode>> IEnumerable<KeyValuePair<string, JsonNode>>.GetEnumerator()
        {
            for (var i = _first; i != None; i = _list[i].Next) {
                yield return _list[i].ToKeyValuePair();
            }
        }

        #endregion

        #region ICollection<string>

        bool ICollection<string>.IsReadOnly
            => true;

        bool ICollection<string>.Contains(string item)
        {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }

            return ContainsKey(item);
        }

        void ICollection<string>.Add(string item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<string>.Remove(string item)
        {
            throw new NotSupportedException();
        }

        void ICollection<string>.Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            CollectionUtil.CopyTo(this, array, arrayIndex);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            for (var i = _first; i != None; i = _list[i].Next) {
                yield return _list[i].Key;
            }
        }

        #endregion

        #region ICollection<JsonNode>

        bool ICollection<JsonNode>.IsReadOnly
            => true;

        void ICollection<JsonNode>.Add(JsonNode item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<JsonNode>.Remove(JsonNode item)
        {
            throw new NotSupportedException();
        }

        void ICollection<JsonNode>.Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<JsonNode>.CopyTo(JsonNode[] array, int arrayIndex)
        {
            CollectionUtil.CopyTo(this, array, arrayIndex);
        }

        bool ICollection<JsonNode>.Contains(JsonNode node)
        {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            return _list.Any(p => p.Value.Equals(node));
        }

        IEnumerator<JsonNode> IEnumerable<JsonNode>.GetEnumerator()
        {
            for (var i = _first; i != None; i = _list[i].Next) {
                yield return _list[i].Value;
            }
        }

        #endregion
    }
}
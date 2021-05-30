using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExactJson.Serialization.Meta
{
    internal class MetaPropertyCollection : KeyedCollection<string, MetaProperty>
    {
        protected override string GetKeyForItem(MetaProperty item)
        {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }

            return item.Name;
        }

        protected override void InsertItem(int index, MetaProperty item)
        {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, MetaProperty item)
        {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }

            base.SetItem(index, item);
        }

        public MetaProperty Find(string name)
        {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (Dictionary != null && Dictionary.TryGetValue(name, out var value)) {
                return value;
            }

            return null;
        }

        public void AddRange(IEnumerable<MetaProperty> items)
        {
            if (items is null) {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items) {
                Add(item);
            }
        }
    }
}
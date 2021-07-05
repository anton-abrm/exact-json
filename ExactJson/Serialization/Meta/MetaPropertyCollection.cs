using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ExactJson.Serialization.Meta
{
    internal class MetaPropertyCollection : KeyedCollection<string, MetaProperty>
    {
        protected override string GetKeyForItem(MetaProperty item)
        {
            Debug.Assert(item is not null);
            
            return item.Name;
        }

        public MetaProperty Find(string name)
        {
            Debug.Assert(name is not null);
            
            if (Dictionary is not null && Dictionary.TryGetValue(name, out var value)) {
                return value;
            }

            return null;
        }

        public void AddRange(IEnumerable<MetaProperty> items)
        {
            Debug.Assert(items is not null);
            
            foreach (var item in items) {
                Add(item);
            }
        }
    }
}
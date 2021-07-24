// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaProperty
    {
        public string Name { get; }
        public MetaType Type { get; }
        public MetaType ParentType { get; }
        
        public Func<object, object> Getter { get; }
        public Func<object, object, object> Setter { get; }

        public MetaContext Context { get;}
        
        public MetaProperty(MetaType type, PropertyInfo propertyInfo, JsonNodeAttribute attribute, MetaType parentType)
        {
            Debug.Assert(type is not null);
            Debug.Assert(propertyInfo is not null);
            Debug.Assert(attribute is not null);
            Debug.Assert(parentType is not null);

            ParentType = parentType;
            Type = type;
            Name = attribute.Name ?? propertyInfo.Name;

            if (propertyInfo.CanRead) {
                Getter = ReflectionUtil.CreatePropertyGetter<object, object>(propertyInfo);
            }
            
            if (propertyInfo.CanWrite) {
                Setter = ReflectionUtil.CreatePropertySetter<object, object>(propertyInfo);
            }
            
            Context = MetaContext.TryCreate(type.UnwrappedType, propertyInfo, JsonNodeTarget.Node);
            
            if (type is IMetaTypeContainer containerType) {
                
                var childContext = MetaContext.TryCreate(containerType.ChildType.UnwrappedType, propertyInfo, JsonNodeTarget.Item);
                
                if (childContext is not null) {

                    if (Context is null) {
                        Context = new MetaContext();
                    }

                    Context.ItemContext = childContext;
                }
            }

            if (type is MetaTypeDictionary dictionaryType) {
                
                var keyContext = MetaContext.TryCreate(dictionaryType.KeyType.UnwrappedType, propertyInfo, JsonNodeTarget.Key);
                
                if (keyContext is not null) {

                    if (Context is null) {
                        Context = new MetaContext();
                    }

                    Context.KeyContext = keyContext;
                }
            }
        }
    }
}
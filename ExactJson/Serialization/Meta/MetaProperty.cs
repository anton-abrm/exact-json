using System;
using System.Reflection;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaProperty
    {
        public string Name { get; }
        public MetaType Type { get; }
        
        public Func<object, object> Getter { get; }
        public Func<object, object, object> Setter { get; }

        public MetaContext Context { get;}
        
        public MetaProperty(MetaType type, PropertyInfo propertyInfo, JsonNodeAttribute attribute)
        {
            if (attribute is null) {
                throw new ArgumentNullException(nameof(attribute));
            }

            Type = type;
            Name = attribute.Name ?? propertyInfo.Name;

            if (!propertyInfo.CanRead) {
                throw new InvalidOperationException($"Property '{propertyInfo.Name}' of type '{type}' has no getter.");
            }
            
            Getter = ReflectionUtil.CreatePropertyGetter<object, object>(propertyInfo);
            
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

                    Context.ChildContext = childContext;
                }
            }

            if (type is MetaTypeDictionary dictionaryType) {
                
                var keyContext = MetaContext.TryCreate(dictionaryType.KeyType.UnwrappedType, propertyInfo, JsonNodeTarget.Key);
                
                if (keyContext is not null) {

                    if (Context is null) {
                        Context = new MetaContext();
                    }

                    Context.ChildKeyContext = keyContext;
                }
            }
        }
    }
}
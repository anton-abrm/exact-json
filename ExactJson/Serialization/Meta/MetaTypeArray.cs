using System;
using System.Collections.Generic;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypeArray : MetaType, IMetaTypeContainer
    {
        public static bool IsArray(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return GetCollectionType(type) != null;
        }
        
        private static Type GetCollectionType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                return type;
            }

            return type.GetInterfaces()
                       .FirstOrDefault(t => t.IsGenericType && 
                                            t.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public override MetaTypeCode MetaCode 
            => MetaTypeCode.Array;

        public MetaType ItemType { get; }
        public Action<object, object> AddInvoker { get; }

        public MetaTypeArray(Type type)
            : base(type)
        {
            var collectionType = GetCollectionType(type);
            if (collectionType is null) {
                throw new ArgumentException($"Type {type} is not array.", nameof(type));
            }

            var itemType = collectionType.GetGenericArguments()[0];

            ItemType = FromType(itemType);

            var clearMethod = collectionType.GetMethod("Clear", Type.EmptyTypes);
            if (clearMethod is null) {
                throw new InvalidOperationException($"Clear method not found in type {type}");
            }

            var addMethod = collectionType.GetMethod("Add", new[] { itemType });
            if (addMethod is null) {
                throw new InvalidOperationException($"Add method not found in type {type}.");
            }
            
            AddInvoker = ReflectionUtil.CreateActionMethodInvoker<object, object>(addMethod);

            var childContext = MetaContext.TryCreate(ItemType.UnwrappedType, UnwrappedType, JsonNodeTarget.Item);

            if (childContext != null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.ChildContext = childContext;
            }
        }

        MetaType IMetaTypeContainer.ChildType 
            => ItemType;
    }
}
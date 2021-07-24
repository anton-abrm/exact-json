// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypeArray : MetaType, IMetaTypeContainer
    {
        public static bool IsArray(Type type)
        {
            Debug.Assert(type is not null);

            return TryGetCollectionType(type) is not null;
        }
        
        private static Type TryGetCollectionType(Type type)
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
            Debug.Assert(type is not null);
            
            var collectionType = TryGetCollectionType(type);

            Debug.Assert(collectionType is not null);
            
            var itemType = collectionType.GetGenericArguments()[0];

            ItemType = FromType(itemType);

            AddInvoker = ReflectionUtil.CreateActionMethodInvoker<object, object>(
                collectionType.GetMethod("Add", new[] { itemType }));

            var childContext = MetaContext.TryCreate(ItemType.UnwrappedType, UnwrappedType, JsonNodeTarget.Item);
            if (childContext is not null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.ItemContext = childContext;
            }
        }

        MetaType IMetaTypeContainer.ChildType 
            => ItemType;
    }
}
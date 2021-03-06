// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal class MetaTypeDictionary : MetaType, IMetaTypeContainer
    {
        public static bool IsDictionary(Type type)
        {
            Debug.Assert(type is not null);
            
            return TryGetDictionaryType(type) is not null;
        }

        private static Type TryGetDictionaryType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) {
                return type;
            }

            return type.GetInterfaces()
                       .FirstOrDefault(t => t.IsGenericType && 
                                            t.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public MetaType KeyType { get; }
        public MetaType ValueType { get; }
        
        public Action<object, object, object> AddInvoker { get; }

        public Func<object, object> KeyGetter { get; }
        public Func<object, object> ValueGetter { get; }
        
        public Func<object, IEnumerator> GetEnumeratorInvoker { get; }

        public MetaTypeDictionary(Type type) : base(type)
        {
            Debug.Assert(type is not null);
            
            var dictionaryType = TryGetDictionaryType(type);
            
            Debug.Assert(dictionaryType is not null);

            var keyType = dictionaryType.GetGenericArguments()[0];
            var valueType = dictionaryType.GetGenericArguments()[1];

            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            
            KeyType = FromType(keyType);
            ValueType = FromType(valueType);
            
            GetEnumeratorInvoker = ReflectionUtil.CreateFuncMethodInvoker<object, IEnumerator>(
                typeof(IEnumerable<>).MakeGenericType(kvpType)
                                     .GetMethod("GetEnumerator", Type.EmptyTypes));

            AddInvoker = ReflectionUtil.CreateActionMethodInvoker<object, object, object>(
                dictionaryType.GetMethod("Add", new[] { keyType, valueType }));

            KeyGetter = ReflectionUtil.CreatePropertyGetter<object, object>(kvpType.GetProperty("Key"));
            ValueGetter = ReflectionUtil.CreatePropertyGetter<object, object>(kvpType.GetProperty("Value"));

            var itemContext = MetaContext.TryCreate(ValueType.UnwrappedType, UnwrappedType, JsonNodeTarget.Item);
            if (itemContext is not null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.ItemContext = itemContext;
            }
            
            var keyContext = MetaContext.TryCreate(KeyType.UnwrappedType, UnwrappedType, JsonNodeTarget.Key);
            if (keyContext is not null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.KeyContext = keyContext;
            }
        }

        public override MetaTypeCode MetaCode
            => MetaTypeCode.Dictionary;

        MetaType IMetaTypeContainer.ChildType 
            => ValueType;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal class MetaTypeDictionary : MetaType, IMetaTypeContainer
    {
        public static bool IsDictionary(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return GetDictionaryType(type) != null;
        }

        private static Type GetDictionaryType(Type type)
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
        
        public Action<object> ClearInvoker { get; }
        public Action<object, object, object> AddInvoker { get; }

        public Func<object, object> KeyGetter { get; }
        public Func<object, object> ValueGetter { get; }
        
        public Func<object, IEnumerator> GetEnumeratorInvoker { get; }

        public MetaTypeDictionary(Type type) : base(type)
        {
            var dictionaryType = GetDictionaryType(type);
            if (dictionaryType is null) {
                throw new ArgumentException($"Type {type} is not dictionary.", nameof(type));
            }

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

            var childContext = MetaContext.TryCreate(ValueType.UnwrappedType, UnwrappedType, JsonNodeTarget.Item);
            if (childContext != null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.ChildContext = childContext;
            }
            
            var keyContext = MetaContext.TryCreate(KeyType.UnwrappedType, UnwrappedType, JsonNodeTarget.Key);
            if (keyContext != null) {

                if (Context is null) {
                    Context = new MetaContext();
                }

                Context.ChildKeyContext = keyContext;
            }
        }

        public override MetaTypeCode MetaCode
            => MetaTypeCode.Dictionary;

        MetaType IMetaTypeContainer.ChildType 
            => ValueType;
    }
}
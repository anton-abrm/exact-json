using System;
using System.Collections.Concurrent;
using System.Diagnostics;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal abstract class MetaType
    {
        protected MetaType(Type type)
        {
            Debug.Assert(type is not null);
            
            IsNullable = ReflectionUtil.IsNullable(type);
            IsNullAssignable = ReflectionUtil.IsNullAssignable(type);
            UnwrappedType = ReflectionUtil.UnwrapNullable(type);

            if (type.IsValueType) {
                Constructor = ReflectionUtil.CreateDefaultConstructor<object>(type);
            }
            else {
                var ci = type.GetConstructor(Type.EmptyTypes);
                if (ci is not null) {
                    Constructor = ReflectionUtil.CreateDefaultConstructor<object>(ci);
                }
            }
            
            Context = MetaContext.TryCreate(UnwrappedType, UnwrappedType, JsonNodeTarget.Node);
        }
        
        public MetaContext Context { get; protected set; }

        public abstract MetaTypeCode MetaCode { get; }

        public bool IsNullable { get; }
        public bool IsNullAssignable { get; }
        public Type UnwrappedType { get; }
        public Func<object> Constructor { get; }

        private static readonly ConcurrentDictionary<Type, MetaType> Cache =
            new ConcurrentDictionary<Type, MetaType>();

        private static MetaType CreateMeta(Type type)
        {
            if (MetaTypeEnum.IsEnum(type)) {
                return new MetaTypeEnum(type);
            }

            if (MetaTypeDictionary.IsDictionary(type)) {
                return new MetaTypeDictionary(type);
            }

            if (MetaTypeArray.IsArray(type)) {
                return new MetaTypeArray(type);
            }

            return new MetaTypeObject(type);
        }

        public static MetaType FromType(Type type)
        {
            Debug.Assert(type is not null);
            
            return MetaTypePrimitive.TryGetPrimitive(type) 
                ?? Cache.GetOrAdd(type, CreateMeta);
        }
    }
}
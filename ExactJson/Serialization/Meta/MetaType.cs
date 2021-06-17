using System;
using System.Collections.Concurrent;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal abstract class MetaType: IEquatable<MetaType>
    {
        protected MetaType(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            IsNullable = ReflectionUtil.IsNullable(type);
            IsNullAssignable = ReflectionUtil.IsNullAssignable(type);
            UnwrappedType = ReflectionUtil.UnwrapNullable(type);

            if (type.IsValueType) {
                Constructor = ReflectionUtil.CreateDefaultConstructor<object>(type);
            }
            else {
                var ci = type.GetConstructor(Type.EmptyTypes);
                if (ci != null) {
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
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return MetaTypePrimitive.TryGetPrimitive(type) 
                ?? Cache.GetOrAdd(type, CreateMeta);
        }

        public override bool Equals(object obj)
            => Equals(obj as MetaType);

        public bool Equals(MetaType other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return UnwrappedType == other.UnwrappedType;
        }
        
        public override int GetHashCode()
            => UnwrappedType.GetHashCode();

        public static bool operator ==(MetaType left, MetaType right)
            => Equals(left, right);

        public static bool operator !=(MetaType left, MetaType right)
            => !Equals(left, right);
    }
}
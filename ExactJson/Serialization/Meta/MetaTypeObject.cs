using System;
using System.Linq;
using System.Reflection;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypeObject : MetaType
    {
        public Func<object> Constructor { get; }

        public MetaPropertyCollection Properties { get; } = new MetaPropertyCollection();

        public override MetaTypeCode MetaCode => MetaTypeCode.Object;

        private static Func<object> TryCreateConstructor(Type type)
        {
            if (type.IsValueType) {
                return ReflectionUtil.CreateDefaultConstructor<object>(type);
            }

            var ci = type.GetConstructor(Type.EmptyTypes);

            if (ci is null) {
                return null;
            }

            return ReflectionUtil.CreateDefaultConstructor<object>(ci);
        }

        public MetaTypeObject(Type type) : base(type)
        {
            if (type.IsAbstract) {
                return;
            }

            Constructor = TryCreateConstructor(type);

            var properties = type.GetProperties(BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.FlattenHierarchy);

            Properties.AddRange(from prop in properties
                                let attr = prop.GetCustomAttributes(true).OfType<JsonNodeAttribute>().FirstOrDefault()
                                where attr != null
                                orderby attr.Position
                                let meta = FromType(prop.PropertyType)
                                select new MetaProperty(meta, prop, attr));
        }
    }
}
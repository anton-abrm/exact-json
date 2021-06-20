using System;
using System.Linq;
using System.Reflection;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypeObject : MetaType
    {
        public MetaPropertyCollection Properties { get; } = new MetaPropertyCollection();

        public override MetaTypeCode MetaCode => MetaTypeCode.Object;

        public MetaTypeObject(Type type) : base(type)
        {
            if (type.IsAbstract) {
                return;
            }

            var properties = type.GetProperties(BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.FlattenHierarchy);

            Properties.AddRange(from prop in properties
                                let attr = prop.GetCustomAttributes(true).OfType<JsonNodeAttribute>().FirstOrDefault()
                                where attr is not null
                                orderby attr.Position
                                let meta = FromType(prop.PropertyType)
                                select new MetaProperty(meta, prop, attr));
        }
    }
}
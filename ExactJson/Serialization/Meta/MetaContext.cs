using System;
using System.Linq;
using System.Reflection;

namespace ExactJson.Serialization.Meta
{
    internal class MetaContext : IMetaContext
    {
        private struct Setup
        {
            public string TypePropertyName { get; set; }
            public string Format { get; set; }
            public IFormatProvider FormatProvider { get; set; }
            public bool? IsOptional { get; set; }
            public bool? IsTuple { get; set; }
            public bool? SerializeNull { get; set; }
            public JsonConverter Converter { get; set; }

            public bool IsEmptySetup
                => TypePropertyName is null &&
                   Format is null &&
                   FormatProvider is null &&
                   IsOptional is null &&
                   IsTuple is null &&
                   SerializeNull is null &&
                   Converter is null;
        }

        public string TypePropertyName { get; }
        public string Format { get; }
        public IFormatProvider FormatProvider { get; }
        public bool? IsOptional { get; }
        public bool? IsTuple { get; }
        public bool? SerializeNull { get; }
        public JsonConverter Converter { get; }

        public MetaContext ChildContext { get; set; }
        public MetaContext ChildKeyContext { get; set; }

        IMetaContext IMetaContext.ChildItem
            => ChildContext;
        
        IMetaContext IMetaContext.ChildKey
            => ChildKeyContext;

        private MetaContext(in Setup setup)
        {
            IsTuple = setup.IsTuple;
            IsOptional = setup.IsOptional;
            SerializeNull = setup.SerializeNull;
            Format = setup.Format;
            FormatProvider = setup.FormatProvider;
            TypePropertyName = setup.TypePropertyName;
            Converter = setup.Converter;
        }

        public MetaContext()
        { }
        
        public static MetaContext TryCreate(Type unwrappedType, MemberInfo memberInfo, JsonNodeTarget target)
        {
            if (unwrappedType is null) {
                throw new ArgumentNullException(nameof(unwrappedType));
            }

            if (memberInfo is null) {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var setup = new Setup();

            setup.IsTuple = memberInfo
                           .GetCustomAttributes(true).OfType<JsonTupleAttribute>()
                           .Where(a => a.ApplyTo == target)
                           .Select(_ => (bool?) true)
                           .FirstOrDefault();

            setup.IsOptional = memberInfo
                              .GetCustomAttributes(true).OfType<JsonNecessityAttribute>()
                              .Where(a => a.ApplyTo == target)
                              .Select(a => (bool?) a.IsOptional)
                              .FirstOrDefault();
            
            setup.SerializeNull = memberInfo
                              .GetCustomAttributes(true).OfType<JsonSerializeNullAttribute>()
                              .Where(a => a.ApplyTo == target)
                              .Select(a => (bool?) a.SerializeNull)
                              .FirstOrDefault();

            setup.Format = memberInfo
                          .GetCustomAttributes(true).OfType<JsonFormatAttribute>()
                          .Where(a => a.ApplyTo == target)
                          .Select(a => a.Format)
                          .FirstOrDefault();
            
            setup.FormatProvider = memberInfo
                                  .GetCustomAttributes(true).OfType<JsonCultureAttribute>()
                                  .Where(a => a.ApplyTo == target)
                                  .Select(a => a.CreateProvider())
                                  .FirstOrDefault();

            setup.TypePropertyName = memberInfo
                                    .GetCustomAttributes(true).OfType<JsonTypePropertyNameAttribute>()
                                    .Where(a => a.ApplyTo == target)
                                    .Select(a => a.PropertyName)
                                    .FirstOrDefault();

            setup.Converter = memberInfo
                             .GetCustomAttributes(true).OfType<JsonConverterAttribute>()
                             .Where(a => a.ApplyTo == target)
                             .Select(a => a.CreateConverter(unwrappedType))
                             .FirstOrDefault();

            if (setup.IsEmptySetup) {
                return null;
            }

            return new MetaContext(setup);
        }
    }
}
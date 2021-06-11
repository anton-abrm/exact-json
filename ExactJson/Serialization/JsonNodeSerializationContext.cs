using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed class JsonNodeSerializationContext : IMetaContext
    {
        public bool? IsTuple { get; set; }
        public bool? IsOptional { get; set; }
        public bool? SerializeNullProperty { get; set; }
        public JsonConverter Converter { get; set; }
        public string Format { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public string TypePropertyName { get; set; }

        public JsonKeySerializationContext KeyContext { get; set; }
        public JsonItemSerializationContext ItemContext { get; set; }
        
        IMetaContext IMetaContext.ChildKey => KeyContext;
        IMetaContext IMetaContext.ChildItem => ItemContext;

        public void SetupKeyContext(Action<JsonKeySerializationContext> setup)
        {
            if (setup is null) {
                throw new ArgumentNullException(nameof(setup));
            }

            var context = KeyContext;
            if (context is null) {
                context = new JsonKeySerializationContext();
                KeyContext = context;
            }

            setup(KeyContext);
        }
        
        public void SetupItemContext(Action<JsonItemSerializationContext> setup)
        {
            if (setup is null) {
                throw new ArgumentNullException(nameof(setup));
            }

            var context = ItemContext;
            if (context is null) {
                context = new JsonItemSerializationContext();
                ItemContext = context;
            }

            setup(ItemContext);
        }
    }
}
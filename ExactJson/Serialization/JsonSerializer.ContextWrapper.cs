using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed partial class JsonSerializer
    {
        private abstract class ContextWrapper : IMetaContext
        {
            protected Type Type { get; }
            protected JsonSerializer Serializer { get; }
            protected JsonNodeSerializationContext NodeContext { get; }

            protected ContextWrapper(JsonSerializer serializer, Type type, JsonNodeSerializationContext context)
            {
                Serializer = serializer;
                Type = type;
                NodeContext = context;
            }

            protected abstract IMetaContext SelectContext(JsonNodeSerializationContext context);

            public abstract IMetaContext KeyContext { get; }
            public abstract IMetaContext ItemContext { get; }
            
            private T GetValue<T>(Func<IMetaContext, T> valueSelector)
            {
                var selfCtx = SelectContext(NodeContext);
                if (selfCtx is not null) {
                    var selfValue = valueSelector(selfCtx);
                    if (selfValue is not null) {
                        return selfValue;
                    }
                }

                for (var type = Type.BaseType; type is not null; type = type.BaseType) {

                    if (!Serializer._contexts.TryGetValue(type, out var wrapper)) {
                        continue;
                    }

                    var ctx = SelectContext(wrapper.NodeContext);
                    if (ctx is null) {
                        continue;
                    }

                    var value = valueSelector(ctx);
                    if (value is null) {
                        continue;
                    }

                    return value;
                }

                return default;
            }

            public bool? IsTuple => GetValue(ctx => ctx.IsTuple);
            public bool? IsOptional => GetValue(ctx => ctx.IsOptional);
            public bool? SerializeNullProperty => GetValue(ctx => ctx.SerializeNullProperty);
            public JsonConverter Converter => GetValue(ctx => ctx.Converter);
            public string Format => GetValue(ctx => ctx.Format);
            public IFormatProvider FormatProvider => GetValue(ctx => ctx.FormatProvider);
            public string TypePropertyName => GetValue(ctx => ctx.TypePropertyName);
        }

        private sealed class NodeContextWrapper : ContextWrapper
        {
            private KeyContextWrapper _keyCtx;
            private ItemContextWrapper _itemCtx;

            public NodeContextWrapper(JsonSerializer serializer, Type type, JsonNodeSerializationContext context)
                : base(serializer, type, context) { }

            protected override IMetaContext SelectContext(JsonNodeSerializationContext context)
                => context;

            public override IMetaContext KeyContext
                => _keyCtx ?? (_keyCtx = new KeyContextWrapper(Serializer, Type, NodeContext));

            public override IMetaContext ItemContext
                => _itemCtx ?? (_itemCtx = new ItemContextWrapper(Serializer, Type, NodeContext));

            public JsonNodeSerializationContext InternalContext
                => NodeContext;
        }

        private sealed class KeyContextWrapper : ContextWrapper
        {
            public KeyContextWrapper(JsonSerializer serializer, Type type, JsonNodeSerializationContext context)
                : base(serializer, type, context) { }

            protected override IMetaContext SelectContext(JsonNodeSerializationContext context)
                => context.KeyContext;

            public override IMetaContext KeyContext => null;
            public override IMetaContext ItemContext => null;
        }

        private sealed class ItemContextWrapper : ContextWrapper
        {
            public ItemContextWrapper(JsonSerializer serializer, Type type, JsonNodeSerializationContext context)
                : base(serializer, type, context) { }

            protected override IMetaContext SelectContext(JsonNodeSerializationContext context)
                => context.ItemContext;
            
            public override IMetaContext KeyContext => null;
            public override IMetaContext ItemContext => null;
        }
    }
}
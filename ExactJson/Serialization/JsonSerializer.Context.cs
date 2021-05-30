using System;

using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed partial class JsonSerializer
    {
        private readonly struct Context
        {
            private readonly IMetaContext _local;
            private readonly IMetaContext _kvAttr;
            private readonly IMetaContext _kvBound;
            private readonly IMetaContext _attr;
            private readonly IMetaContext _bound;
            
            public static Context Local(IMetaContext ctx)
                => new Context(ctx);

            public Context Item()
                => new Context(_local?.ChildItem, _attr?.ChildItem, _bound?.ChildItem);

            public Context Key()
                => new Context(_local?.ChildKey, _attr?.ChildKey, _bound?.ChildKey);

            public Context SetType(JsonSerializer serializer, MetaType meta)
            {
                NodeContextWrapper boundContext = null;
                
                for (var curr = meta.UnwrappedType; curr != null; curr = curr.BaseType) {
                    if (serializer._contexts.TryGetValue(curr, out boundContext)) {
                       break;
                    }
                }
                
                return new Context(_local, _kvAttr, _kvBound, meta.Context, boundContext);
            }
            
            private Context(IMetaContext local = null,
                            IMetaContext kvAttr = null,
                            IMetaContext kvBound = null,
                            IMetaContext attr = null,
                            IMetaContext bound = null)
            {
                _local = local;
                _kvAttr = kvAttr;
                _kvBound = kvBound;
                _attr = attr;
                _bound = bound;
            }

            public bool IsOptional(JsonSerializer serializer)
                => _local?.IsOptional ??
                   _kvAttr?.IsOptional ??
                   _kvBound?.IsOptional ??
                   _attr?.IsOptional ??
                   _bound?.IsOptional ?? serializer.IsNodeOptional;

            public bool IsTuple(JsonSerializer serializer)
                => _local?.IsTuple ??
                   _kvAttr?.IsTuple ??
                   _kvBound?.IsTuple ??
                   _attr?.IsTuple ??
                   _bound?.IsTuple ?? serializer.IsNodeTuple;
            
            public bool SerializeNull(JsonSerializer serializer)
                => _local?.SerializeNull ??
                   _kvAttr?.SerializeNull ??
                   _kvBound?.SerializeNull ??
                   _attr?.SerializeNull ??
                   _bound?.SerializeNull ?? serializer.SerializeNull;

            public string GetFormat()
                => _local?.Format ??
                   _kvAttr?.Format ??
                   _kvBound?.Format ??
                   _attr?.Format ??
                   _bound?.Format;

            public JsonConverter GetConverter()
                => _local?.Converter ??
                   _kvAttr?.Converter ??
                   _kvBound?.Converter ??
                   _attr?.Converter ??
                   _bound?.Converter;

            public IFormatProvider GetFormatProvider(JsonSerializer serializer)
                => _local?.FormatProvider ??
                   _kvAttr?.FormatProvider ??
                   _kvBound?.FormatProvider ??
                   _attr?.FormatProvider ??
                   _bound?.FormatProvider ?? serializer.FormatProvider;

            public string GetTypePropertyName(JsonSerializer serializer)
                => _local?.TypePropertyName ??
                   _kvAttr?.TypePropertyName ??
                   _kvBound?.TypePropertyName ??
                   _attr?.TypePropertyName ??
                   _bound?.TypePropertyName ?? serializer.TypePropertyName;
        }
    }
}
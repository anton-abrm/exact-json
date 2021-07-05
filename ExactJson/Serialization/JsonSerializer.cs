using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using ExactJson.Infra;
using ExactJson.Serialization.Meta;

namespace ExactJson.Serialization
{
    public sealed partial class JsonSerializer
    {
        private readonly Dictionary<string, Type> _typeAliases
            = new Dictionary<string, Type>();

        private readonly Dictionary<Type, string> _typeAliasesReverse
            = new Dictionary<Type, string>();

        private readonly HashSet<Type> _baseTypes = new HashSet<Type>();
        private readonly Dictionary<Type, NodeContextWrapper> _contexts = new Dictionary<Type, NodeContextWrapper>();

        private string _typePropertyName = "__type";
        private IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
        
        public string TypePropertyName
        {
            get => _typePropertyName;
            set => _typePropertyName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IFormatProvider FormatProvider
        {
            get => _formatProvider;
            set => _formatProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool SerializeNullProperty { get; set; }

        public bool IsNodeOptional { get; set; }

        public bool IsNodeTuple { get; set; }

        public void RegisterType(Type type, string alias)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (alias is null) {
                throw new ArgumentNullException(nameof(alias));
            }

            var meta = MetaType.FromType(type) as MetaTypeObject;

            if (meta is null || meta.IsNullable) {
                throw new ArgumentException($"Type '{type}' not supported.", nameof(type));
            }

            _baseTypes.UnionWith(meta.UnwrappedType.GetInterfaces());
            _baseTypes.UnionWith(meta.UnwrappedType.GetBaseTypes());

            _typeAliases[alias] = meta.UnwrappedType;
            _typeAliasesReverse[meta.UnwrappedType] = alias;
        }

        public void SetContext(Type type, JsonNodeSerializationContext context)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (context is null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (ReflectionUtil.IsNullable(type)) {
                throw new ArgumentException("Type can not be nullable.", nameof(type));
            }

            _contexts[type] = new NodeContextWrapper(this, type, context);
        }

        public JsonNodeSerializationContext GetContext(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (_contexts.TryGetValue(type, out var ctx)) {
                return ctx.InternalContext;
            }

            return null;
        }

        private static void CheckType(MetaType type)
        {
            if (type.Constructor is null) {
                throw new JsonInvalidTypeException($"Constructor on type '{type.UnwrappedType}' not found.");
            }
        }
        
        private static void CheckProperty(MetaProperty property)
        {
            if (property.Getter is null) {
                throw new JsonInvalidTypeException($"Type '{property.ParentType.UnwrappedType}' has property '{property.Name}' with no getter.");
            }
        }

        #region Serialize

        public void Serialize(Type type, JsonWriter writer, object value, JsonNodeSerializationContext context)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }
            
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }
            
            var targetType = MetaType.FromType(type);

            var stack = new Stack<PointerSection>();

            var ctx = Context.Local(context)
                             .SetType(this, targetType);

            try {
                SerializeAnyChecked(writer, value, ctx, stack, targetType, null);
            }
            catch (JsonException ex) {

                var pointer = stack.Reverse()
                                   .Aggregate(JsonPointer.Root(), (p, s) => s.AttachTo(p));

                throw new JsonSerializationException(pointer, ex);
            }
        }

        private void SerializeAnyChecked(JsonWriter writer, object value, Context ctx, Stack<PointerSection> stack, MetaType targetType, string propertyName)
        {
            if (value is null) {

                if (!ctx.IsOptional(this)) {
                    throw new JsonMissingRequiredValueException();
                }

                if (propertyName is null) {
                    writer.WriteNull();
                }
                else if (ctx.SerializeNullProperty(this)) {
                    writer.WriteProperty(propertyName);
                    writer.WriteNull();
                }

                return;
            }

            if (propertyName is not null) {
                writer.WriteProperty(propertyName);
            }

            var converter = ctx.GetConverter();
            if (converter is not null) {
                
                writer.WriteString(converter.GetString(value, new JsonConverterContext {
                    Format = ctx.GetFormat(),
                    FormatProvider = ctx.GetFormatProvider(this),
                    TargetType = targetType.UnwrappedType,
                }));

                return;
            }
            
            var valueType = MetaType.FromType(value.GetType());

            if (targetType.MetaCode != valueType.MetaCode || 
                targetType.MetaCode != MetaTypeCode.Object && valueType.UnwrappedType != targetType.UnwrappedType) {
                throw new JsonInvalidTypeException(
                    $"Unable to serialize '{valueType.UnwrappedType}' as '{targetType.UnwrappedType}'.");
            }

            switch (targetType.MetaCode) {

                case MetaTypeCode.Enum:
                    SerializeEnum(writer, (MetaTypeEnum) targetType, (Enum) value);
                    return;

                case MetaTypeCode.Primitive:
                    SerializePrimitive(writer, (MetaTypePrimitive) targetType, value, ctx);
                    return;

                case MetaTypeCode.Array:
                    SerializeArray(writer, (MetaTypeArray) targetType, (IEnumerable) value, ctx, stack);
                    return;
                
                case MetaTypeCode.Dictionary:
                    SerializeDictionary(writer, (MetaTypeDictionary) targetType, (IEnumerable) value, ctx, stack);
                    return;

                case MetaTypeCode.Object:
                    SerializeObject(writer, (MetaTypeObject) targetType, (MetaTypeObject) valueType, value, ctx, stack);
                    return;
            }
        }

        private void SerializeObject(JsonWriter writer, MetaTypeObject targetType, MetaTypeObject valueType, object value, Context ctx, Stack<PointerSection> stack)
        {
            CheckType(valueType);
            
            if (ctx.IsTuple(this)) {

                writer.WriteStartArray();

                WriteTypeAliasIfNeeded(false);
                
                for (var index = 0; index < valueType.Properties.Count; index++) {

                    var property = valueType.Properties[index];

                    CheckProperty(property);
                    
                    var childValue = property.Getter(value);

                    var childCtx = Context.Local(property.Context)
                                          .SetType(this, property.Type);

                    stack.Push(new PointerSection(index));
                    SerializeAnyChecked(writer, childValue, childCtx, stack, property.Type, null);
                    stack.Pop();
                }

                writer.WriteEndArray();
            }
            else {

                writer.WriteStartObject();

                WriteTypeAliasIfNeeded(true);

                foreach (var property in valueType.Properties) {

                    CheckProperty(property);
                    
                    var childValue = property.Getter(value);

                    var childCtx = Context.Local(property.Context)
                                          .SetType(this, property.Type);
                    
                    stack.Push(new PointerSection(property.Name));
                    SerializeAnyChecked(writer, childValue, childCtx, stack, property.Type, property.Name);
                    stack.Pop();
                }

                writer.WriteEndObject();
            }
            
            void WriteTypeAliasIfNeeded(bool writeProperty)
            {
                if (targetType.UnwrappedType == valueType.UnwrappedType) {
                    return;
                }

                if (!_typeAliasesReverse.TryGetValue(valueType.UnwrappedType, out var alias)) {
                    throw new JsonInvalidTypeException($"Type {valueType.UnwrappedType} must have alias.");
                }

                if (writeProperty) {
                    writer.WriteProperty(ctx.GetTypePropertyName(this));
                }
                
                writer.WriteString(alias);
            }
        }

        private void SerializeDictionary(JsonWriter writer, MetaTypeDictionary targetType, IEnumerable value, Context ctx, Stack<PointerSection> stack)
        {
            CheckType(targetType);
            
            writer.WriteStartObject();

            var keyTypeEnum = targetType.KeyType as MetaTypeEnum;

            var enumerator = targetType.GetEnumeratorInvoker(value);

            var keyCtx = ctx.Key()
                            .SetType(this, targetType.KeyType);

            var converter = keyCtx.GetConverter();
            
            try {

                while (enumerator.MoveNext()) {

                    var kvp = enumerator.Current;

                    var key = targetType.KeyGetter(kvp);
                    
                    string name;

                    if (converter is not null) {
                        name = converter.GetString(key, new JsonConverterContext {
                            Format = keyCtx.GetFormat(),
                            FormatProvider = keyCtx.GetFormatProvider(this),
                            TargetType = targetType.KeyType.UnwrappedType
                        });
                    }
                    else if (keyTypeEnum is not null) {
                        name = keyTypeEnum.TryGetName((Enum) key);
                    }
                    else {
                        name = (string) key;
                    }

                    if (name is null) {
                        throw new JsonInvalidValueException();
                    }
                    
                    var childValue = targetType.ValueGetter(kvp);

                    var childCtx = ctx.Item()
                                      .SetType(this, targetType.ValueType);

                    stack.Push(new PointerSection(name));
                    SerializeAnyChecked(writer, childValue, childCtx, stack, targetType.ValueType, name);
                    stack.Pop();
                }
            }
            finally {
                if (enumerator is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            writer.WriteEndObject();
        }

        private void SerializeArray(JsonWriter writer, MetaTypeArray type, IEnumerable value, Context ctx, Stack<PointerSection> stack)
        {
            CheckType(type);
            
            writer.WriteStartArray();

            var index = 0;

            var itemCtx = ctx.Item()
                             .SetType(this, type.ItemType);

            foreach (var itemValue in value) {
                
                stack.Push(new PointerSection(index));
                SerializeAnyChecked(writer, itemValue, itemCtx, stack, type.ItemType, null);
                stack.Pop();
                
                index++;
            }

            writer.WriteEndArray();
        }

        private static void SerializeEnum(JsonWriter writer, MetaTypeEnum type, Enum value)
        {
            var name = type.TryGetName(value);
            if (name is null) {
                throw new JsonInvalidValueException();
            }
            
            writer.WriteString(name);
        }

        private static void SerializePrimitive(JsonWriter writer, MetaTypePrimitive type, object value, Context ctx)
        {
            switch (type.PrimitiveCode) {

                case MetaTypePrimitiveCode.Boolean:
                    writer.WriteBool((bool) value);
                    return;

                case MetaTypePrimitiveCode.String:
                    writer.WriteString((string) value);
                    return;

                case MetaTypePrimitiveCode.SByte:
                    writer.WriteNumber((sbyte) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Int16:
                    writer.WriteNumber((short) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Int32:
                    writer.WriteNumber((int) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Int64:
                    writer.WriteNumber((long) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Byte:
                    writer.WriteNumber((byte) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.UInt16:
                    writer.WriteNumber((ushort) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.UInt32:
                    writer.WriteNumber((uint) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.UInt64:
                    writer.WriteNumber((ulong) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Single:
                    writer.WriteNumber((float) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Double:
                    writer.WriteNumber((double) value, ctx.GetFormat());
                    return;

                case MetaTypePrimitiveCode.Decimal:
                    writer.WriteNumber((decimal) value, ctx.GetFormat());
                    return;
            }
        }

        #endregion

        #region Deserialize

        public object Deserialize(Type type, JsonReader reader, JsonNodeSerializationContext context = null)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (reader is null) {
                throw new ArgumentNullException(nameof(reader));
            }

            var targetType = MetaType.FromType(type);

            var ctx = Context.Local(context)
                             .SetType(this, targetType);

            var stack = new Stack<PointerSection>();

            try {

                if (reader.MoveToToken() == JsonTokenType.None) {
                    throw new EndOfStreamException();
                }

                return DeserializeAnyChecked(reader, targetType, ctx, stack);
            }
            catch (JsonException ex) {

                var pointer = stack.Reverse()
                                   .Aggregate(JsonPointer.Root(), (p, s) => s.AttachTo(p));

                throw new JsonSerializationException(pointer, ex);
            }
        }

        private object DeserializeAnyChecked(JsonReader reader, MetaType targetType, Context ctx, Stack<PointerSection> stack)
        {
            var result = DeserializeAny(reader, targetType, ctx, stack);

            if (result is null) {
                if (!ctx.IsOptional(this) || !targetType.IsNullAssignable) {
                    throw new JsonMissingRequiredValueException();
                }
            }

            return result;
        }

        private object DeserializeAny(JsonReader reader, MetaType targetType, Context ctx, Stack<PointerSection> stack)
        {
            var converter = ctx.GetConverter();
            if (converter is not null) {
                return converter.GetValue(reader.ReadString(), new JsonConverterContext {
                    Format = ctx.GetFormat(),
                    FormatProvider = ctx.GetFormatProvider(this),
                    TargetType = targetType.UnwrappedType,
                });
            }

            switch (reader.TokenType) {

                case JsonTokenType.StartObject when targetType.MetaCode == MetaTypeCode.Dictionary:
                    return DeserializeDictionary(reader, (MetaTypeDictionary) targetType, ctx, stack);

                case JsonTokenType.StartObject:
                    return DeserializeObject(reader, targetType, ctx, stack);

                case JsonTokenType.StartArray:
                    return DeserializeArray(reader, targetType, ctx, stack);

                case JsonTokenType.Null:
                    return DeserializeNull(reader);

                case JsonTokenType.Bool:
                    return DeserializeBool(reader, targetType);

                case JsonTokenType.String:
                    return DeserializeString(reader, targetType);

                case JsonTokenType.Number:
                    return DeserializeNumber(reader, targetType);

                default:
                    throw new InvalidOperationException($"Unexpected reader token '{reader.TokenType}'.");
            }
        }

        private object DeserializeDictionary(JsonReader reader, MetaTypeDictionary dictType, Context ctx, Stack<PointerSection> stack)
        {
            CheckType(dictType);
            
            var result = dictType.Constructor();

            reader.ReadStartObject();

            var keyCtx = ctx.Key()
                            .SetType(this, dictType.KeyType);

            var keyConverter = keyCtx.GetConverter();

            var keyTypeEnum = dictType.KeyType as MetaTypeEnum;

            while (reader.TokenType != JsonTokenType.EndObject) {

                var propertyName = reader.ReadProperty();

                object key;

                if (keyConverter is not null) {
                    key = keyConverter.GetValue(propertyName, new JsonConverterContext {
                        Format = keyCtx.GetFormat(),
                        FormatProvider = keyCtx.GetFormatProvider(this),
                        TargetType = dictType.KeyType.UnwrappedType
                    });
                }
                else if (keyTypeEnum is not null) {
                    key = keyTypeEnum.TryGetValue(keyTypeEnum.UnwrappedType, propertyName);
                }
                else {
                    key = propertyName;
                }

                if (key is null) {
                    throw new JsonInvalidValueException();
                }

                var itemCtx = ctx.Item()
                                 .SetType(this, dictType.ValueType);

                stack.Push(new PointerSection(propertyName));
                var itemResult = DeserializeAnyChecked(reader, dictType.ValueType, itemCtx, stack);
                stack.Pop();

                dictType.AddInvoker(result, key, itemResult);
            }

            reader.ReadEndObject();

            return result;
        }

        private object DeserializeObject(JsonReader reader, MetaType targetType, Context ctx, Stack<PointerSection> stack)
        {
            var meta = targetType as MetaTypeObject;
            if (meta is null) {
                throw new JsonInvalidTypeException();
            }

            var typePropertyName = ctx.GetTypePropertyName(this);

            string typeAlias = null;

            if (_baseTypes.Contains(meta.UnwrappedType)) {

                if (reader.CanSaveState) {

                    var state = reader.SaveState();

                    if (reader.MoveToProperty(typePropertyName)) {
                        reader.ReadProperty();

                        if (reader.TokenType != JsonTokenType.String) {
                            throw new JsonInvalidTypeException();
                        }

                        typeAlias = reader.ReadString();
                    }

                    state.Restore();

                    reader.ReadStartObject();
                }
                else {

                    reader.ReadStartObject();

                    if (reader.TokenType == JsonTokenType.Property &&
                        reader.ValueAsString == typePropertyName) {

                        reader.ReadProperty();

                        if (reader.TokenType != JsonTokenType.String) {
                            throw new JsonInvalidTypeException();
                        }

                        typeAlias = reader.ReadString();
                    }
                }

                if (typeAlias is not null) {

                    if (!_typeAliases.TryGetValue(typeAlias, out var type)) {
                        throw new JsonInvalidTypeException();
                    }

                    meta = (MetaTypeObject) MetaType.FromType(type);
                }
            }
            else {
                reader.ReadStartObject();
            }

            CheckType(meta);

            var result = meta.Constructor();

            var processed = new Dictionary<string, object>();

            while (reader.TokenType != JsonTokenType.EndObject) {

                var name = reader.ReadProperty();

                if (name == typePropertyName) {

                    if (reader.TokenType == JsonTokenType.String) {

                        var value = reader.ReadString();

                        if (typeAlias is not null && typeAlias == value) {
                            continue;
                        }

                        if (!_typeAliases.TryGetValue(value, out var type)) {
                            continue;
                        }

                        if (meta.UnwrappedType == type) {
                            continue;
                        }
                    }

                    throw new JsonInvalidTypeException();
                }
                
                var property = meta.Properties.Find(name);
                if (property is not null) {
                    CheckProperty(property);
                }
                
                stack.Push(new PointerSection(name));
                
                if (property is null || property.Setter is null) {
                    reader.Skip();
                }
                else {
                    var childCtx = Context.Local(property.Context)
                        .SetType(this, property.Type);
                    
                    processed.Add(name, DeserializeAny(reader, property.Type, childCtx, stack));
                }
                
                stack.Pop();
            }

            reader.ReadEndObject();

            foreach (var property in meta.Properties) {

                CheckProperty(property);
                
                if (property.Setter is null) {
                    continue;
                }

                processed.TryGetValue(property.Name, out var value);

                var childCtx = Context.Local(property.Context)
                                      .SetType(this, property.Type);

                stack.Push(new PointerSection(property.Name));

                if (value is null) {
                    if (!childCtx.IsOptional(this) || !property.Type.IsNullAssignable) {
                        throw new JsonMissingRequiredValueException();
                    }
                }

                stack.Pop();

                result = property.Setter(result, value);
            }

            return result;
        }

        private object DeserializeArray(JsonReader reader, MetaType targetType, Context ctx, Stack<PointerSection> stack)
        {
            switch (targetType) {

                case MetaTypeArray metaArray:
                {
                    CheckType(metaArray);

                    var result = metaArray.Constructor();

                    reader.ReadStartArray();

                    var itemCtx = ctx.Item().SetType(this, metaArray.ItemType);

                    for (var i = 0; reader.TokenType != JsonTokenType.EndArray; i++) {

                        stack.Push(new PointerSection(i));
                        var value = DeserializeAnyChecked(reader, metaArray.ItemType, itemCtx, stack);
                        stack.Pop();

                        metaArray.AddInvoker(result, value);
                    }

                    reader.ReadEndArray();

                    return result;
                }

                case MetaTypeObject meta when ctx.IsTuple(this):
                {
                    reader.ReadStartArray();

                    if (reader.TokenType == JsonTokenType.String) {
                        if (_typeAliases.TryGetValue(reader.ValueAsString, out var type)) {
                            meta = (MetaTypeObject) MetaType.FromType(type);
                            reader.Read();
                        }
                    }

                    CheckType(meta);

                    var result = meta.Constructor();

                    int index = 0;

                    while (reader.TokenType != JsonTokenType.EndArray) {

                        var property = meta.Properties[index];
                        
                        CheckProperty(property);

                        stack.Push(new PointerSection(index));

                        if (property.Setter is null) {
                            reader.Skip();
                        }
                        else {
                            var itemCtx = ctx.Item().SetType(this, property.Type);
                            result = property.Setter(result, DeserializeAnyChecked(reader, property.Type, itemCtx, stack));
                        }

                        stack.Pop();
                        
                        index++;
                    }

                    reader.ReadEndArray();

                    if (index != meta.Properties.Count) {
                        throw new JsonInvalidValueException();
                    }

                    return result;
                }

                default:
                    throw new JsonInvalidTypeException();
            }
        }

        private static object DeserializeNull(JsonReader reader)
        {
            reader.ReadNull();
            return null;
        }

        private static object DeserializeBool(JsonReader reader, MetaType targetType)
        {
            switch (targetType) {

                case MetaTypePrimitive primitive when primitive.PrimitiveCode == MetaTypePrimitiveCode.Boolean:
                    return reader.ReadBool();

                default:
                    throw new JsonInvalidTypeException();
            }
        }

        private static object DeserializeString(JsonReader reader, MetaType targetType)
        {
            switch (targetType) {

                case MetaTypePrimitive primitive when primitive.PrimitiveCode == MetaTypePrimitiveCode.String:
                    return reader.ReadString();

                case MetaTypeEnum enumType:
                    return enumType.TryGetValue(targetType.UnwrappedType, reader.ReadString())
                        ?? throw new JsonInvalidValueException();

                default:
                    throw new JsonInvalidTypeException();
            }
        }

        private static object DeserializeNumber(JsonReader reader, MetaType targetType)
        {
            var primitiveType = targetType as MetaTypePrimitive;
            if (primitiveType is null) {
                throw new JsonInvalidTypeException();
            }

            try {

                switch (primitiveType.PrimitiveCode) {

                    case MetaTypePrimitiveCode.SByte:
                        return (sbyte) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Int16:
                        return (short) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.UInt16:
                        return (ushort) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Int32:
                        return (int) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Byte:
                        return (byte) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.UInt32:
                        return (uint) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Int64:
                        return (long) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.UInt64:
                        return (ulong) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Single:
                        return (float) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Double:
                        return (double) reader.ReadNumber(out _);

                    case MetaTypePrimitiveCode.Decimal:
                        return (decimal) reader.ReadNumber(out _);

                    default:
                        throw new JsonInvalidTypeException();
                }
            }
            catch (OverflowException) {
                throw new JsonValueOutOfRangeException();
            }
        }

        #endregion
    }
}
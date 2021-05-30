using System;
using System.Collections.Generic;
using System.Linq;

using ExactJson.Infra;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypeEnum : MetaType
    {
        private readonly Dictionary<Enum, string> _itemsForward =
            new Dictionary<Enum, string>();

        private readonly Dictionary<string, Enum> _itemsBackward =
            new Dictionary<string, Enum>();

        public override MetaTypeCode MetaCode => MetaTypeCode.Enum;

        public MetaTypeEnum(Type type) : base(type)
        {
            type = ReflectionUtil.UnwrapNullable(type);
            
            if (!type.IsEnum) {
                throw new ArgumentException($"Type {type} is not enum.", nameof(type));
            }

            foreach (var field in type.GetFields()) {

                if (!field.IsStatic) {
                    continue;
                }

                var attr = field.GetCustomAttributes(false)
                                .OfType<JsonEnumValueAttribute>()
                                .FirstOrDefault();

                if (attr is null) {
                    continue;
                }

                var name = attr.Name ?? field.Name;

                var value = (Enum) field.GetValue(null);

                _itemsForward[value] = name;
                _itemsBackward[name] = value;
            }
        }

        public string GetName(Enum value)
        {
            if (_itemsForward.TryGetValue(value, out string name)) {
                return name;
            }

            throw new InvalidOperationException($"Attribute for {value} not found.");
        }

        public object TryGetValue(Type type, string name)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (_itemsBackward.TryGetValue(name, out var value)) {
                return value;
            }

            return null;
        }

        public static bool IsEnum(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }
            
            return ReflectionUtil.UnwrapNullable(type).IsEnum;
        }
    }
}
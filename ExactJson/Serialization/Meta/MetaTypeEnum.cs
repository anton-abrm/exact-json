// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Assert(type is not null);
            
            type = ReflectionUtil.UnwrapNullable(type);
            
            Debug.Assert(type.IsEnum);
            
            foreach (var field in type.GetFields()) {

                if (!field.IsStatic) {
                    continue;
                }

                var attribute = field.GetCustomAttributes(false)
                                .OfType<JsonEnumValueAttribute>()
                                .FirstOrDefault();

                if (attribute is null) {
                    continue;
                }

                var name = attribute.Name ?? field.Name;

                var value = (Enum) field.GetValue(null);

                _itemsForward[value] = name;
                _itemsBackward[name] = value;
            }
        }

        public string TryGetName(Enum value)
        {
            _itemsForward.TryGetValue(value, out string name);
            return name;
        }

        public object TryGetValue(Type type, string name)
        {
            Debug.Assert(type is not null);
            Debug.Assert(name is not null);
            
            _itemsBackward.TryGetValue(name, out var value);
            return value;
        }

        public static bool IsEnum(Type type)
        {
            Debug.Assert(type is not null);
            
            return ReflectionUtil.UnwrapNullable(type).IsEnum;
        }
    }
}
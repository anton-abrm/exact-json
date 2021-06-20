using System;
using System.Collections.Generic;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonNumberConverter : JsonStringConverter
    {
        private static readonly Dictionary<Type, TypeCode> Map = new Dictionary<Type, TypeCode> {
            [typeof(byte)] = TypeCode.Byte,
            [typeof(sbyte)] = TypeCode.SByte,
            [typeof(short)] = TypeCode.Int16,
            [typeof(ushort)] = TypeCode.UInt16,
            [typeof(int)] = TypeCode.Int32,
            [typeof(uint)] = TypeCode.UInt32,
            [typeof(long)] = TypeCode.Int64,
            [typeof(ulong)] = TypeCode.UInt64,
            [typeof(decimal)] = TypeCode.Decimal,
            [typeof(float)] = TypeCode.Single,
            [typeof(double)] = TypeCode.Double,
        };

        private static string GetString(byte value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(sbyte value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(short value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(ushort value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(int value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(uint value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(long value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(ulong value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(decimal value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(float value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        private static string GetString(double value, string format)
        {
            return ((JsonDecimal) value).ToString(format != null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }


        private static string GetString(IConvertible value, string format)
        {
            switch (value.GetTypeCode()) {

                case TypeCode.Byte:
                    return GetString((byte) value, format);

                case TypeCode.Decimal:
                    return GetString((decimal) value, format);

                case TypeCode.Double:
                    return GetString((double) value, format);

                case TypeCode.Int16:
                    return GetString((short) value, format);

                case TypeCode.Int32:
                    return GetString((int) value, format);

                case TypeCode.Int64:
                    return GetString((long) value, format);

                case TypeCode.SByte:
                    return GetString((sbyte) value, format);

                case TypeCode.Single:
                    return GetString((float) value, format);

                case TypeCode.UInt16:
                    return GetString((ushort) value, format);

                case TypeCode.UInt32:
                    return GetString((uint) value, format);

                case TypeCode.UInt64:
                    return GetString((ulong) value, format);

                default:
                    throw new ArgumentException("Invalid value type.", nameof(value));
            }
        }

        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return GetString((IConvertible) value, context.Format);
        }
        
        private static object GetValue(JsonDecimal value, Type targetType)
        {
            Map.TryGetValue(targetType, out var typeCode);

            switch (typeCode) {

                case TypeCode.Byte:
                    return (byte) value;

                case TypeCode.Decimal:
                    return (decimal) value;

                case TypeCode.Double:
                    return (double) value;

                case TypeCode.Int16:
                    return (short) value;

                case TypeCode.Int32:
                    return (int) value;

                case TypeCode.Int64:
                    return (long) value;

                case TypeCode.SByte:
                    return (sbyte) value;

                case TypeCode.Single:
                    return (float) value;

                case TypeCode.UInt16:
                    return (ushort) value;

                case TypeCode.UInt32:
                    return (uint) value;

                case TypeCode.UInt64:
                    return (ulong) value;

                default:
                    throw new ArgumentException($"Target type '{targetType}' not supported.", nameof(targetType));
            }
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return GetValue(JsonDecimal.Parse(s), context.TargetType);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
            catch (OverflowException) {
                throw new JsonValueOutOfRangeException();
            }
        }
        
        public override object Read(JsonReader input, JsonConverterContext context)
        {
            if (input is null) {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.TokenType == JsonTokenType.Number) {
                return GetValue(input.ReadNumber(out _), context.TargetType);
            }

            return base.Read(input, context);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExactJson.Serialization.Meta
{
    internal sealed class MetaTypePrimitive : MetaType
    {
        private static readonly Dictionary<Type, MetaTypePrimitive> Primitives =
            new Dictionary<Type, MetaTypePrimitive>();

        public static MetaTypePrimitive TryGetPrimitive(Type type)
        {
            Debug.Assert(type is not null);
            
            Primitives.TryGetValue(type, out var result);

            return result;
        }

        static MetaTypePrimitive()
        {
            Primitives.Add(typeof(string), new MetaTypePrimitive(MetaTypePrimitiveCode.String, typeof(string)));

            Primitives.Add(typeof(bool), new MetaTypePrimitive(MetaTypePrimitiveCode.Boolean, typeof(bool)));
            Primitives.Add(typeof(bool?), new MetaTypePrimitive(MetaTypePrimitiveCode.Boolean, typeof(bool?)));

            Primitives.Add(typeof(sbyte), new MetaTypePrimitive(MetaTypePrimitiveCode.SByte, typeof(sbyte)));
            Primitives.Add(typeof(sbyte?), new MetaTypePrimitive(MetaTypePrimitiveCode.SByte, typeof(sbyte?)));

            Primitives.Add(typeof(byte), new MetaTypePrimitive(MetaTypePrimitiveCode.Byte, typeof(byte)));
            Primitives.Add(typeof(byte?), new MetaTypePrimitive(MetaTypePrimitiveCode.Byte, typeof(byte?)));

            Primitives.Add(typeof(short), new MetaTypePrimitive(MetaTypePrimitiveCode.Int16, typeof(short)));
            Primitives.Add(typeof(short?), new MetaTypePrimitive(MetaTypePrimitiveCode.Int16, typeof(short?)));

            Primitives.Add(typeof(ushort), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt16, typeof(ushort)));
            Primitives.Add(typeof(ushort?), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt16, typeof(ushort?)));

            Primitives.Add(typeof(int), new MetaTypePrimitive(MetaTypePrimitiveCode.Int32, typeof(int)));
            Primitives.Add(typeof(int?), new MetaTypePrimitive(MetaTypePrimitiveCode.Int32, typeof(int?)));

            Primitives.Add(typeof(uint), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt32, typeof(uint)));
            Primitives.Add(typeof(uint?), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt32, typeof(uint?)));

            Primitives.Add(typeof(long), new MetaTypePrimitive(MetaTypePrimitiveCode.Int64, typeof(long)));
            Primitives.Add(typeof(long?), new MetaTypePrimitive(MetaTypePrimitiveCode.Int64, typeof(long?)));

            Primitives.Add(typeof(ulong), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt64, typeof(ulong)));
            Primitives.Add(typeof(ulong?), new MetaTypePrimitive(MetaTypePrimitiveCode.UInt64, typeof(ulong?)));

            Primitives.Add(typeof(float), new MetaTypePrimitive(MetaTypePrimitiveCode.Single, typeof(float)));
            Primitives.Add(typeof(float?), new MetaTypePrimitive(MetaTypePrimitiveCode.Single, typeof(float?)));

            Primitives.Add(typeof(double), new MetaTypePrimitive(MetaTypePrimitiveCode.Double, typeof(double)));
            Primitives.Add(typeof(double?), new MetaTypePrimitive(MetaTypePrimitiveCode.Double, typeof(double?)));

            Primitives.Add(typeof(decimal), new MetaTypePrimitive(MetaTypePrimitiveCode.Decimal, typeof(decimal)));
            Primitives.Add(typeof(decimal?), new MetaTypePrimitive(MetaTypePrimitiveCode.Decimal, typeof(decimal?)));
        }

        private MetaTypePrimitive(MetaTypePrimitiveCode primitiveCode, Type type)
            : base(type)
        {
            PrimitiveCode = primitiveCode;
        }

        public override MetaTypeCode MetaCode => MetaTypeCode.Primitive;

        public MetaTypePrimitiveCode PrimitiveCode { get; }
    }
}
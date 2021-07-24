// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonCharReaderTests
    {
        private static object FilterNumberAsJsonDecimal(object value)
        {
            if (value is IConvertible conv) {

                switch (conv.GetTypeCode()) {

                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        return (JsonDecimal) Convert.ToInt64(value);

                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return (JsonDecimal) Convert.ToUInt64(value);

                    case TypeCode.Single:
                    case TypeCode.Double:
                        return (JsonDecimal) Convert.ToDouble(value);

                    case TypeCode.Decimal:
                        return (JsonDecimal) Convert.ToDecimal(value);
                }
            }

            return value;
        }

        private static JsonCharReader CreateReader(Type readerType, string json)
        {
            if (readerType == typeof(JsonStringReader)) {
                return new JsonStringReader(json);
            }
            
            if (readerType == typeof(JsonStreamReader)) {
                return new JsonStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(json))) {
                    CloseInput = true
                };
            }

            if (readerType == typeof(JsonTextReader)) {
                return new JsonTextReader(new StringReader(json)) {
                    CloseInput = true
                };
            }
            
            throw new ArgumentOutOfRangeException(nameof(readerType));
        }
        
        [Test]
        public void Read_InvalidTokenPrefix_ThrowsJsonTextReaderException()
        {
            using var jr = new JsonStringReader("a");

            var ex = Assert.Throws<JsonCharReaderException>(() =>
            {
                jr.Read();
            });

            Assert.That(ex.LineNumber, Is.EqualTo(1));
            Assert.That(ex.LinePosition, Is.EqualTo(1));
        }

        #region Read Sibling Nodes at Zero Depth

        [Test]
        [TestCase("nullnull", new[] { JsonTokenType.Null, JsonTokenType.Null })]
        [TestCase(" null \n null ", new[] { JsonTokenType.Null, JsonTokenType.Null })]
        [TestCase("falsefalse", new[] { JsonTokenType.Bool, JsonTokenType.Bool })]
        [TestCase(" false \n false ", new[] { JsonTokenType.Bool, JsonTokenType.Bool })]
        [TestCase("truetrue", new[] { JsonTokenType.Bool, JsonTokenType.Bool })]
        [TestCase(" true \n true ", new[] { JsonTokenType.Bool, JsonTokenType.Bool })]
        [TestCase("123 456", new[] { JsonTokenType.Number, JsonTokenType.Number })]
        [TestCase(" 123 \n 456 ", new[] { JsonTokenType.Number, JsonTokenType.Number })]
        [TestCase("\"123\"\"456\"", new[] { JsonTokenType.String, JsonTokenType.String })]
        [TestCase(" \"123\" \n \"456\" ", new[] { JsonTokenType.String, JsonTokenType.String })]
        [TestCase("[][]", new[] {
            JsonTokenType.StartArray, JsonTokenType.EndArray,
            JsonTokenType.StartArray, JsonTokenType.EndArray,
        })]
        [TestCase(" [] \n [] ", new[] {
            JsonTokenType.StartArray, JsonTokenType.EndArray,
            JsonTokenType.StartArray, JsonTokenType.EndArray,
        })]
        [TestCase("{}{}", new[] {
            JsonTokenType.StartObject, JsonTokenType.EndObject,
            JsonTokenType.StartObject, JsonTokenType.EndObject,
        })]
        [TestCase(" {} \n {} ", new[] {
            JsonTokenType.StartObject, JsonTokenType.EndObject,
            JsonTokenType.StartObject, JsonTokenType.EndObject,
        })]
        [TestCase("123[]null{}false\"hello\"", new[] {
            JsonTokenType.Number,
            JsonTokenType.StartArray, JsonTokenType.EndArray,
            JsonTokenType.Null,
            JsonTokenType.StartObject, JsonTokenType.EndObject,
            JsonTokenType.Bool,
            JsonTokenType.String,
        })]
        [TestCase(" 123 \n[]\nnull {} false\n\n   \"hello\" ", new[] {
            JsonTokenType.Number,
            JsonTokenType.StartArray, JsonTokenType.EndArray,
            JsonTokenType.Null,
            JsonTokenType.StartObject, JsonTokenType.EndObject,
            JsonTokenType.Bool,
            JsonTokenType.String,
        })]
        public void Read_GivenNodesAtDepthZero_ReadsTokens(string json, JsonTokenType[] expected)
        {
            using var jr = new JsonStringReader(json);

            var tokens = new List<JsonTokenType>();

            while (jr.Read()) {
                tokens.Add(jr.TokenType);
            }

            Assert.That(tokens, Is.EqualTo(expected));
        }

        #endregion

        #region Read String

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(1024)]
        [TestCase(1025)]
        [TestCase(2048)]
        public void Read_StringOfNChars(int charCount)
        {
            var expected = new string('a', charCount);

            var sb = new StringBuilder();

            sb.Append('"');
            sb.Append(expected);
            sb.Append('"');

            var json = sb.ToString();

            using var jr = new JsonStringReader(json);

            jr.Read();

            Assert.That(jr.ValueAsString, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("\"\"", "")]
        [TestCase("\"foo\"", "foo")]
        [TestCase("\"foo\\u0020bar\"", "foo bar")]
        [TestCase("\"foo\\nbar\"", "foo\nbar")]
        [TestCase("\"foo\\rbar\"", "foo\rbar")]
        [TestCase("\"foo\\tbar\"", "foo\tbar")]
        [TestCase("\"foo\\bbar\"", "foo\bbar")]
        [TestCase("\"foo\\fbar\"", "foo\fbar")]
        [TestCase("\"foo\\\\bar\"", "foo\\bar")]
        [TestCase("\"foo\\/bar\"", "foo/bar")]
        [TestCase("\"\\\"foo\\\"\"", "\"foo\"")]
        public void Read_GivenString_ReadsString(string json, string expected)
        {
            using var jr = new JsonStringReader(json);

            jr.Read();

            Assert.That(jr.ValueAsString, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("\"foo\\xbar\"", 1, 6)]
        [TestCase("\"foo\nbar\"", 1, 5)]
        [TestCase("\"foo\\ubar\"", 1, 9)]
        public void Read_GivenInvalidString_ThrowsJsonTextReaderException(string json, int lineNumber, int linePosition)
        {
            using var jr = new JsonStringReader(json);

            var ex = Assert.Throws<JsonCharReaderException>(() => jr.Read());

            Assert.That(ex.LineNumber, Is.EqualTo(lineNumber));
            Assert.That(ex.LinePosition, Is.EqualTo(linePosition));
        }

        [Test]
        [TestCase("\"Foo")]
        public void Read_GivenStringWithoutEndQuote_ThrowsEndOfStreamException(string json)
        {
            using var jr = new JsonStringReader(json);

            Assert.Throws<EndOfStreamException>(() =>
            {
                jr.Read();
            });
        }

        #endregion

        #region Read Null

        [Test]
        [TestCase("nul")]
        public void Read_GivenIncompleteNull_ThrowsEndOfStreamException(string json)
        {
            using var jr = new JsonStringReader(json);

            Assert.Throws<EndOfStreamException>(() => jr.Read());
        }

        [Test]
        [TestCase("nula")]
        public void Read_GivenInvalidNull_ThrowsJsonTextReaderException(string json)
        {
            using var jr = new JsonStringReader(json);

            var ex = Assert.Throws<JsonCharReaderException>(() => jr.Read());

            Assert.AreEqual(1, ex.LineNumber);
            Assert.AreEqual(4, ex.LinePosition);
        }

        [Test]
        [TestCase("null", 1, 5)]
        [TestCase("   null", 1, 8)]
        [TestCase("null   ", 1, 5)]
        [TestCase("   null   ", 1, 8)]
        public void Read_GivenNull_ReadsNull(string json, int lineNumber, int linePosition)
        {
            using var jr = new JsonStringReader(json);

            jr.Read();

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));

            Assert.That(jr.LineNumber, Is.EqualTo(lineNumber));
            Assert.That(jr.LinePosition, Is.EqualTo(linePosition));
        }

        #endregion

        #region Read Bool

        [Test]
        [TestCase("tru")]
        [TestCase("fals")]
        public void Read_GivenIncompleteBool_ThrowsEndOfStreamException(string json)
        {
            using var jr = new JsonStringReader(json);

            Assert.Throws<EndOfStreamException>(() => jr.Read());
        }

        [Test]
        [TestCase("trua", 1, 4)]
        [TestCase("falsa", 1, 5)]
        public void Read_GivenInvalidBool_ThrowsJsonTextReaderException(string json, int lineNumber, int linePosition)
        {
            using var jr = new JsonStringReader(json);

            var ex = Assert.Throws<JsonCharReaderException>(() => jr.Read());

            Assert.That(ex.LineNumber, Is.EqualTo(lineNumber));
            Assert.That(ex.LinePosition, Is.EqualTo(linePosition));
        }

        [Test]
        [TestCase("true", true, 1, 5)]
        [TestCase("   true", true, 1, 8)]
        [TestCase("true   ", true, 1, 5)]
        [TestCase("   true   ", true, 1, 8)]
        [TestCase("false", false, 1, 6)]
        [TestCase("   false", false, 1, 9)]
        [TestCase("false   ", false, 1, 6)]
        [TestCase("   false   ", false, 1, 9)]
        public void Read_GivenBool_ReadsBool(string json, bool expected, int lineNumber, int linePosition)
        {
            using var jr = new JsonStringReader(json);

            jr.Read();

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Bool));
            Assert.That(jr.ValueAsBool, Is.EqualTo(expected));

            Assert.That(jr.LineNumber, Is.EqualTo(lineNumber));
            Assert.That(jr.LinePosition, Is.EqualTo(linePosition));
        }

        #endregion

        #region Read Numbers

        private static readonly object[] NumbersWithinDecimalRange = {
            new object[] { "-79228162514264337593543950335", -79228162514264337593543950335M },
            new object[] { "-7.9228162514264337593543950335e+28", -7.9228162514264337593543950335e+28M },
            new object[] { "-9223372036854775809", -9223372036854775809M },
            new object[] { "-9.223372036854775809e+18", -9.223372036854775809e+18M },
            new object[] { "-9223372036854775808", -9223372036854775808M },
            new object[] { "-2147483649", -2147483649M },
            new object[] { "-2147483648", -2147483648M },
            new object[] { "-1", -1M },
            new object[] { "0", 0M },
            new object[] { "0e+0", 0M },
            new object[] { "1", 1M },
            new object[] { "2147483647", 2147483647M },
            new object[] { "2147483648", 2147483648M },
            new object[] { "9223372036854775807", 9223372036854775807M },
            new object[] { "9223372036854775808", 9223372036854775808M },
            new object[] { "9.223372036854775808e+18", 9.223372036854775808e+18M },
            new object[] { "79228162514264337593543950335", 79228162514264337593543950335M },
            new object[] { "7.9228162514264337593543950335e+28", 7.9228162514264337593543950335e+28M },

            new object[] { "-0.0000000000000000000000000001", -0.0000000000000000000000000001M },
            new object[] { "0.0000000000000000000000000001", 0.0000000000000000000000000001M },
        };

        // TODO: Implement outside of Decimal128 range
        // [Test]
        // [TestCase("-1.79769313486231E+308", -1.79769313486231E+308, 1e+294)]
        // [TestCase("1.79769313486231E+308", 1.79769313486231E+308, 1e+294)]
        // [TestCase("-7.9228162514264337593543950336e+28", -7.92281625142643E+28, 1e+14)]
        // [TestCase("7.9228162514264337593543950336e+28", 7.92281625142643E+28, 1e+14)]
        // [TestCase("-79228162514264337593543950336", -7.92281625142643E+28, 1e+14)]
        // [TestCase("79228162514264337593543950336", 7.92281625142643E+28, 1e+14)]
        // [TestCase("-1E-323", -1E-323, 1E-323)]
        // [TestCase("-0.00000000000000000000000000001", -1E-29,1E-29 )]
        // [TestCase("0.00000000000000000000000000001", 1E-29, 1E-29)]
        // [TestCase("1E-323", 1E-323,  1E-323)]
        // [TestCase("79228162514264337593543950335.1", 7.92281625142643E+28, 1e+14)]
        // public void Read_GivenNumbersOutOfDecimalRange_ThrowsJsonTextReaderException(string json, double expectedValue, double delta) {
        //     
        //     using var tr = new StringReader(json);
        //     using var jr = new JsonTextReader(tr);
        //
        //     Assert.Throws<JsonTextReaderException>(() => jr.Read());
        // }

        [Test]
        [TestCaseSource(nameof(NumbersWithinDecimalRange))]
        public void Read_GivenNumbersWithinDecimalRange_ReadsDecimal(string json, decimal expectedValue)
        {
            using var jr = new JsonStringReader(json);

            jr.Read();

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Number));
            Assert.That(jr.ValueAsNumber, Is.EqualTo((JsonDecimal) expectedValue));
        }

        #endregion

        #region Read Array

        [Test]
        [TestCase("[")]
        public void Read_GivenArrayWithoutEndBraked_ThrowsEndOfStreamException(string json)
        {
            using var jr = new JsonStringReader(json);

            Assert.Throws<EndOfStreamException>(delegate
            {
                while (jr.Read()) { }
            });
        }

        [Test]
        [TestCase("[\"a\"\"b\"]", 1, 5)]
        public void Read_GivenArrayWithUnexpectedSymbol_ThrowsJsonTextReaderException(string json, int row, int col)
        {
            using var jr = new JsonStringReader(json);

            var ex = Assert.Throws<JsonCharReaderException>(delegate
            {
                while (jr.Read()) { }
            });

            Assert.That(ex.LineNumber, Is.EqualTo(row));
            Assert.That(ex.LinePosition, Is.EqualTo(col));
        }

        [Test]
        [TestCase("[]")]
        [TestCase(" [ ] ")]
        public void Read_GivenEmptyArray_ReadsArray(string json)
        {
            using var jr = new JsonStringReader(json);

            var list = new List<Tuple<JsonTokenType, object>>();

            while (jr.Read()) {
                list.Add(new Tuple<JsonTokenType, object>(jr.TokenType, jr.Value));
            }

            Assert.That(list, Is.EqualTo(new[] {
                new Tuple<JsonTokenType, object>(JsonTokenType.StartArray, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndArray, null),
            }));
        }

        [Test]
        [TestCase("[ 1,\"a\",null,true,{},[] ]")]
        [TestCase("[ 1,\"a\",null,true,{},[],]")]
        public void Read_GivenArrayWithSixItems_ReadsArray(string json)
        {
            using var jr = new JsonStringReader(json);

            var list = new List<Tuple<JsonTokenType, object>>();

            while (jr.Read()) {
                list.Add(new Tuple<JsonTokenType, object>(jr.TokenType, jr.Value));
            }

            Assert.That(list, Is.EqualTo(new[] {
                new Tuple<JsonTokenType, object>(JsonTokenType.StartArray, null),

                new Tuple<JsonTokenType, object>(JsonTokenType.Number, (JsonDecimal) 1),
                new Tuple<JsonTokenType, object>(JsonTokenType.String, "a"),
                new Tuple<JsonTokenType, object>(JsonTokenType.Null, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.Bool, true),

                new Tuple<JsonTokenType, object>(JsonTokenType.StartObject, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndObject, null),

                new Tuple<JsonTokenType, object>(JsonTokenType.StartArray, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndArray, null),

                new Tuple<JsonTokenType, object>(JsonTokenType.EndArray, null),
            }));
        }

        #endregion

        #region Read Object

        [Test]
        [TestCase("{\"a\"\"b\"}", 1, 5)]
        [TestCase("{\"a\":\"b\"\"c\":\"d\"}", 1, 9)]
        [TestCase("{a\":\"b\"}", 1, 2)]
        public void Read_GivenObjectWithUnexpectedSymbol_ThrowsJsonTextReaderException(string json, int row, int col)
        {
            using var jr = new JsonStringReader(json);

            var ex = Assert.Throws<JsonCharReaderException>(delegate
            {
                while (jr.Read()) { }
            });

            Assert.That(ex.LineNumber, Is.EqualTo(row));
            Assert.That(ex.LinePosition, Is.EqualTo(col));
        }

        [Test]
        [TestCase("{")]
        public void Read_GivenObjectWithoutEndBraked_ThrowsEndOfStreamException(string json)
        {
            using var jr = new JsonStringReader(json);

            Assert.Throws<EndOfStreamException>(delegate
            {
                while (jr.Read()) { }
            });
        }

        [Test]
        [TestCase("{}")]
        [TestCase(" { } ")]
        public void Read_GivenEmptyObject_ReadsObject(string json)
        {
            using var jr = new JsonStringReader(json);

            var list = new List<Tuple<JsonTokenType, object>>();

            while (jr.Read()) {
                list.Add(new Tuple<JsonTokenType, object>(jr.TokenType, jr.Value));
            }

            Assert.That(list, Is.EqualTo(new[] {
                new Tuple<JsonTokenType, object>(JsonTokenType.StartObject, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndObject, null),
            }));
        }

        [Test]
        [TestCase("{ \"n1\": 1,\"n2\": \"a\",\"n3\": null,\"n4\": true,\"n5\": {},\"n6\": []  }")]
        [TestCase("{ \"n1\": 1,\"n2\": \"a\",\"n3\": null,\"n4\": true,\"n5\": {},\"n6\": [], }")]
        public void Read_GivenObjectWithSixProperties_ReadsObject(string json)
        {
            using var jr = new JsonStringReader(json);

            var list = new List<Tuple<JsonTokenType, object>>();

            while (jr.Read()) {
                list.Add(new Tuple<JsonTokenType, object>(jr.TokenType, jr.Value));
            }

            Assert.That(list, Is.EqualTo(new[] {
                new Tuple<JsonTokenType, object>(JsonTokenType.StartObject, null),

                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n1"),
                new Tuple<JsonTokenType, object>(JsonTokenType.Number, (JsonDecimal) 1),
                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n2"),
                new Tuple<JsonTokenType, object>(JsonTokenType.String, "a"),
                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n3"),
                new Tuple<JsonTokenType, object>(JsonTokenType.Null, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n4"),
                new Tuple<JsonTokenType, object>(JsonTokenType.Bool, true),
                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n5"),
                new Tuple<JsonTokenType, object>(JsonTokenType.StartObject, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndObject, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.Property, "n6"),
                new Tuple<JsonTokenType, object>(JsonTokenType.StartArray, null),
                new Tuple<JsonTokenType, object>(JsonTokenType.EndArray, null),

                new Tuple<JsonTokenType, object>(JsonTokenType.EndObject, null),
            }));
        }

        #endregion

        #region ValueAsBool

        [Test]
        [TestCase("", JsonTokenType.None)]
        [TestCase("null", JsonTokenType.Null)]
        [TestCase("1", JsonTokenType.Number)]
        [TestCase("\"foo\"", JsonTokenType.String)]
        [TestCase("{", JsonTokenType.StartObject)]
        [TestCase("{}", JsonTokenType.EndObject)]
        [TestCase("[", JsonTokenType.StartArray)]
        [TestCase("[]", JsonTokenType.EndArray)]
        [TestCase("{ \"foo\": ", JsonTokenType.Property)]
        public void ValueAsBool_ReadNotBool_ThrowsInvalidOperationException(string json, JsonTokenType tokenType)
        {
            using var jr = new JsonStringReader(json);

            while (jr.Read() && jr.TokenType != tokenType) { }

            Assert.That(jr.TokenType, Is.EqualTo(tokenType));

            Assert.Throws<InvalidOperationException>(() =>
            {
                bool _ = jr.ValueAsBool;
            });
        }

        #endregion

        #region ValueAsString

        [Test]
        [TestCase("", JsonTokenType.None)]
        [TestCase("null", JsonTokenType.Null)]
        [TestCase("true", JsonTokenType.Bool)]
        [TestCase("1", JsonTokenType.Number)]
        [TestCase("{", JsonTokenType.StartObject)]
        [TestCase("{}", JsonTokenType.EndObject)]
        [TestCase("[", JsonTokenType.StartArray)]
        [TestCase("[]", JsonTokenType.EndArray)]
        public void ValueAsString_ReadNotStringOrProperty_ThrowsInvalidOperationException(string json,
                                                                                          JsonTokenType tokenType)
        {
            using var jr = new JsonStringReader(json);

            while (jr.Read() && jr.TokenType != tokenType) { }

            Assert.That(jr.TokenType, Is.EqualTo(tokenType));

            Assert.Throws<InvalidOperationException>(() =>
            {
                string _ = jr.ValueAsString;
            });
        }

        #endregion

        #region ValueAsDecimal

        [Test]
        [TestCase("", JsonTokenType.None)]
        [TestCase("null", JsonTokenType.Null)]
        [TestCase("true", JsonTokenType.Bool)]
        [TestCase("\"foo\"", JsonTokenType.String)]
        [TestCase("{", JsonTokenType.StartObject)]
        [TestCase("{}", JsonTokenType.EndObject)]
        [TestCase("[", JsonTokenType.StartArray)]
        [TestCase("[]", JsonTokenType.EndArray)]
        [TestCase("{ \"foo\": ", JsonTokenType.Property)]
        public void ValueAsDecimal_ReadNotDecimal_ThrowsInvalidOperationException(string json, JsonTokenType tokenType)
        {
            using var jr = new JsonStringReader(json);

            while (jr.Read() && jr.TokenType != tokenType) { }

            Assert.That(jr.TokenType, Is.EqualTo(tokenType));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jr.ValueAsNumber;
            });
        }

        [Test]
        public void Read_InvalidNumber_ThrowsJsonReaderException()
        {
            using var jr = new JsonStringReader("-2-34");

            Assert.Throws<JsonCharReaderException>(() => jr.Read());
        }

        #endregion

        #region SaveState

        [TestCase(typeof(JsonStreamReader))]
        [TestCase(typeof(JsonStringReader))]
        public void SaveAndRestoreState_Edges(Type readerType)
        {
            using var jr = CreateReader(readerType, "[]");

            Assert.That(jr.TokenType == JsonTokenType.None);
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(1));

            var begin = jr.SaveState();

            while (jr.Read()) { }
            
            Assert.That(jr.TokenType == JsonTokenType.None);
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(3));
            
            var end = jr.SaveState();
            
            begin.Restore();

            Assert.That(jr.TokenType == JsonTokenType.None);
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(1));
            
            end.Restore();
            
            Assert.That(jr.TokenType == JsonTokenType.None);
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(3));
        }
        
        [TestCase(typeof(JsonStreamReader), JsonTokenType.String, "foo")]
        [TestCase(typeof(JsonStreamReader), JsonTokenType.Bool, true)]
        [TestCase(typeof(JsonStreamReader), JsonTokenType.Number, 1)]
        [TestCase(typeof(JsonStringReader), JsonTokenType.String, "foo")]
        [TestCase(typeof(JsonStringReader), JsonTokenType.Bool, true)]
        [TestCase(typeof(JsonStringReader), JsonTokenType.Number, 1)]
        public void SaveAndRestoreState(Type readerType, JsonTokenType tokenType, object value)
        {
            value = FilterNumberAsJsonDecimal(value);

            var json = "[\"foo\", true, 1]";

            using var jr = new JsonStringReader(json);

            Assert.That(jr.MoveToToken(), Is.EqualTo(JsonTokenType.StartArray));

            while (jr.Read() && jr.TokenType != tokenType) { }

            Assert.That(jr.TokenType, Is.EqualTo(tokenType));
            Assert.That(jr.Value, Is.EqualTo(value));

            var lineNumber = jr.LineNumber;
            var linePosition = jr.LinePosition;

            var state = jr.SaveState();

            while (jr.Read() && jr.TokenType != JsonTokenType.EndArray) { }

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndArray));

            state.Restore();

            Assert.That(lineNumber, Is.EqualTo(lineNumber));
            Assert.That(linePosition, Is.EqualTo(linePosition));

            Assert.That(jr.TokenType, Is.EqualTo(tokenType));
            Assert.That(jr.Value, Is.EqualTo(value));
        }

        [Test]
        public void SaveState_NotSupported_ThrowsNotSupportedException()
        {
            using var sr = new StringReader("[]");
            using var jr = new JsonTextReader(sr);

            Assert.Throws<NotSupportedException>(() => jr.SaveState());
        }
        
        [Test]
        public void SaveState_ValuesAtRoot()
        {
            using var jr = new JsonStringReader("1 2 3 4 5");

            jr.MoveToToken();

            Assert.That(jr.ReadNumber(), Is.EqualTo((JsonDecimal) 1));
            Assert.That(jr.ReadNumber(), Is.EqualTo((JsonDecimal) 2));
            
            Assert.That(jr.Value, Is.EqualTo((JsonDecimal) 3));
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Number));
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(6));

            var state = jr.SaveState();
            
            Assert.That(jr.ReadNumber(), Is.EqualTo((JsonDecimal) 3));
            Assert.That(jr.ReadNumber(), Is.EqualTo((JsonDecimal) 4));
            Assert.That(jr.ReadNumber(), Is.EqualTo((JsonDecimal) 5));
            
            state.Restore();
            
            Assert.That(jr.Value, Is.EqualTo((JsonDecimal) 3));
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Number));
            Assert.That(jr.LineNumber, Is.EqualTo(1));
            Assert.That(jr.LinePosition, Is.EqualTo(6));
        }
        
        #endregion

        [TestCase(typeof(JsonTextReader))]
        [TestCase(typeof(JsonStringReader))]
        [TestCase(typeof(JsonStreamReader))]
        public void Read_EndOfStream_ThrowsEndOfStreamException(Type readerType)
        {
            using var reader = CreateReader(readerType, "[");

            reader.Read();
            
            Assert.Throws<EndOfStreamException>(() => reader.Read());
        }
    }
}
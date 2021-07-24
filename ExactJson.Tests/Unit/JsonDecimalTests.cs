// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonDecimal128Tests
    {
        private static readonly object[] RoundTrip_Source =
        {
            new object[] { "-0", "-" },
            new object[] { "0", "" },

            new object[] { "0", "" },
            new object[] { "0.0", ".1" },
            new object[] { "0e0", "e" },
            new object[] { "0.0e0", ".1e" },
            new object[] { "0.0e00", ".1e2" },

            new object[] { "1", "" },
            new object[] { "1234567890123456789012345678901234", "" },
            new object[] { "9999999999999999999999999999999999", "" },

            // --- Max ---
            new object[]
                { "9999999999999999999999999999999999e6111", "e,34" },
            new object[]
                { "9.999999999999999999999999999999999e6144", "e" },

            // --- Min ---
            new object[] { "1e-6143", "e" },
            new object[] { "1000000000000000000000000000000000e-6176", "e,34" },

            new object[] { "1.1", "" },
            new object[] { "1.01", "" },
            new object[] { "10.01", "" },
            new object[] { "1.00000000000000001", "" },
            new object[] { "1.000000000000000000000000000000001", "" },

            new object[] { "1.1e10", "e" },
            new object[] { "1.01e10", "e" },
            new object[] { "1.00000000000000001e10", "e" },
            new object[] { "1.000000000000000000000000000000001e10", "e" },

            new object[] { "0.1", "" },
            new object[] { "0.01", "" },
            new object[] { "0.00000000000000001", "" },
            new object[] { "0.000000000000000000000000000000001", "" },

            new object[] { "0.1e1", "e,0" },
            new object[] { "0.01e1", "e,-1" },
            new object[] { "0.001e1", "e,-2" },

            new object[] { "0.1", "" },
            new object[] { "123", "" },
            new object[] { "123.45", "" },
            new object[] { "0123.45", "4" },
            new object[] { "0123.450", "4.3" },
            new object[] { "100000000000000000000000000000000000000000000000000", "" },

            new object[]
            {
                "100000000000000000000000000000000000000000000000000" +
                "00000000000000000000000000000000000000000000000000",
                "",
            },

            new object[] { "0.00000000000000000000000000000000000000000000000001", "" },

            new object[]
            {
                "0.00000000000000000000000000000000000000000000000000" +
                "00000000000000000000000000000000000000000000000001",
                "",
            },

            // --- Exponential ---
            new object[] { "1.23e8", "e" },
            new object[] { "123e10", "e,3" },

            new object[] { "1E10", "E" },
            new object[] { "1e+10", "e+" },
            new object[] { "1E+10", "E+" },
        };

        private static readonly object[] Cast_Decimal_Source =
        {
            new object[] { 0m, "0" },
            new object[] { -1e-28m, "-1e-28" },
            new object[] { 1e-28m, "1e-28" },
            new object[] { -79228162514264337593543950335m, "-7.9228162514264337593543950335e28" },
            new object[] { 79228162514264337593543950335m, "7.9228162514264337593543950335e28" },
            new object[] { -7.9228162514264337593543950335m, "-7.9228162514264337593543950335" },
            new object[] { 7.9228162514264337593543950335m, "7.9228162514264337593543950335" },
        };

        [TestCaseSource(nameof(RoundTrip_Source))]
        public void RoundTrip(string input, string expectedFormat)
        {
            var num = JsonDecimal.Parse(input, out var fmt);

            Assert.That(fmt.ToString(), Is.EqualTo(expectedFormat));
            Assert.That(num.ToString(fmt), Is.EqualTo(input));
        }

        #region Casting

        [TestCase((sbyte) 127)]
        [TestCase((sbyte) 126)]
        [TestCase((sbyte) 0)]
        [TestCase((sbyte) -127)]
        [TestCase((sbyte) -128)]
        public void Cast_Int08(sbyte source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (sbyte) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(32767)]
        [TestCase(32766)]
        [TestCase(0)]
        [TestCase(-32767)]
        [TestCase(-32768)]
        public void Cast_Int16(short source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (short) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(2147483647)]
        [TestCase(2147483646)]
        [TestCase(0)]
        [TestCase(-2147483647)]
        [TestCase(-2147483648)]
        public void Cast_Int32(int source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (int) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(9223372036854775807L)]
        [TestCase(9223372036854775806L)]
        [TestCase(0L)]
        [TestCase(-9223372036854775807L)]
        [TestCase(-9223372036854775808L)]
        public void Cast_Int64(long source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (long) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(255)]
        [TestCase(254)]
        [TestCase(1)]
        [TestCase(0)]
        public void Cast_UInt08(byte source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (byte) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase((ushort) 65535)]
        [TestCase((ushort) 65534)]
        [TestCase((ushort) 1U)]
        [TestCase((ushort) 0U)]
        public void Cast_UInt16(ushort source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (ushort) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(4294967295U)]
        [TestCase(4294967294U)]
        [TestCase(1U)]
        [TestCase(0U)]
        public void Cast_UInt32(uint source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (uint) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(18446744073709551615UL)]
        [TestCase(18446744073709551614UL)]
        [TestCase(1UL)]
        [TestCase(0UL)]
        public void Cast_UInt64(ulong source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (ulong) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase("128")]
        [TestCase("-129")]
        public void Cast_Int08_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (sbyte) d128;
            });
        }

        [TestCase("256")]
        [TestCase("-1")]
        public void Cast_UInt08_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (byte) d128;
            });
        }

        [TestCase("32768")]
        [TestCase("-32769")]
        public void Cast_Int16_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (short) d128;
            });
        }

        [TestCase("65536")]
        [TestCase("-1")]
        public void Cast_UInt16_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (ushort) d128;
            });
        }

        [TestCase("2147483648")]
        [TestCase("-2147483649")]
        public void Cast_Int32_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (int) d128;
            });
        }

        [TestCase("4294967296")]
        [TestCase("-1")]
        public void Cast_UInt32_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (uint) d128;
            });
        }

        [TestCase("100000000000000000000")]
        [TestCase("9223372036854775808")]
        [TestCase("1.1")]
        [TestCase("0.1")]
        [TestCase("-9223372036854775809")]
        public void Cast_Int64_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (long) d128;
            });
        }

        [TestCase("100000000000000000000")]
        [TestCase("18446744073709551616")]
        [TestCase("1.1")]
        [TestCase("0.1")]
        [TestCase("-1")]
        public void Cast_UInt64_Overflow_ThrowsOverflowException(string value)
        {
            var d128 = JsonDecimal.Parse(value);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (ulong) d128;
            });
        }

        [TestCaseSource(nameof(Cast_Decimal_Source))]
        public void Cast_Decimal(decimal input, string expectedString)
        {
            var converted = (JsonDecimal) input;
            var expected = JsonDecimal.Parse(expectedString);

            Assert.That(converted, Is.EqualTo(expected));

            var restored = (decimal) converted;

            Assert.That(restored, Is.EqualTo(input));
        }

        [TestCase("79228162514264337593543950336")]
        [TestCase("-79228162514264337593543950336")]
        [TestCase("1e29")]
        [TestCase("1e-29")]
        public void Cast_Decimal_Overflow_ThrowsOverflowException(string s)
        {
            var d128 = JsonDecimal.Parse(s);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (decimal) d128;
            });
        }

        [TestCase(float.Epsilon)]
        [TestCase(float.MinValue)]
        [TestCase(float.MaxValue)]
        [TestCase(0.0F)]
        public void Cast_Single(float source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (float) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(double.Epsilon)]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(0.0)]
        public void Cast_Double(double source)
        {
            var d128 = (JsonDecimal) source;
            var restored = (double) d128;

            Assert.That(restored, Is.EqualTo(source));
        }

        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Cast_Double_NotFinite_ThrowsOverflowException(double d)
        {
            Assert.Throws<OverflowException>(() =>
            {
                var _ = (JsonDecimal) d;
            });
        }

        [TestCase(float.NaN)]
        [TestCase(float.NegativeInfinity)]
        [TestCase(float.PositiveInfinity)]
        public void Cast_Single_NotFinite_ThrowsOverflowException(float f)
        {
            Assert.Throws<OverflowException>(() =>
            {
                var _ = (JsonDecimal) f;
            });
        }

        #endregion

        [TestCase("0", "-0", true)]
        [TestCase("0e1", "0e0", true)]
        [TestCase("1e1", "1e0", false)]
        [TestCase("1", "1", true)]
        [TestCase("1", "0", false)]
        public void Equality(string s1, string s2, bool expectedEquals)
        {
            var d1 = JsonDecimal.Parse(s1);
            var d2 = JsonDecimal.Parse(s2);

            bool result = d1.Equals((object) d2);

            Assert.That(result, Is.EqualTo(expectedEquals));
            Assert.That(d1 == d2, Is.EqualTo(expectedEquals));
            Assert.That(d1 != d2, Is.Not.EqualTo(expectedEquals));

            if (expectedEquals) {
                Assert.That(d1.GetHashCode(), Is.EqualTo(d2.GetHashCode()));
            }
        }

        [Test]
        [TestCase("0", 0, true)]
        [TestCase("1", 0, true)]
        [TestCase("1", 1, true)]
        [TestCase("1", 28, true)]
        [TestCase("1.0000000000000000000000000001", 28, true)]
        [TestCase("1.0000000000000000000000000001", 27, false)]
        [TestCase("1.1", 0, false)]
        [TestCase("1.1", 1, true)]
        public void HasPrecision(string s, int precision, bool expectedResult)
        {
            var value = JsonDecimal.Parse(s);
            bool result = JsonDecimal.HasPrecision(value, precision);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void HasPrecision_OutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                JsonDecimal.HasPrecision(new JsonDecimal(), -1);
            });
        }

        [Test]
        [TestCase("0", true)]
        [TestCase("0.1", false)]
        [TestCase("1", true)]
        [TestCase("1.1", false)]
        [TestCase("1e33", true)]
        [TestCase("1e34", true)]
        [TestCase("1234567890123456789012345678901234", true)]
        public void IsInteger(string valueString, bool expected)
        {
            var value = JsonDecimal.Parse(valueString);
            bool result = JsonDecimal.IsInteger(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AppendTo_NullBuilder_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new JsonDecimal().AppendTo(null, JsonNumberFormat.Decimal);
            });
        }


        [TestCase("9", "1e1", -1)]
        [TestCase("1", "2", -1)]
        [TestCase("1", "1", 0)]
        [TestCase("0.1", "1", -1)]
        [TestCase("0", "1", -1)]
        [TestCase("0", "0", 0)]
        [TestCase("0", "-1", 1)]
        [TestCase("-0.1", "-1", 1)]
        [TestCase("-1", "-1", 0)]
        [TestCase("-1", "-2", 1)]
        [TestCase("-9", "-1e1", 1)]
        [TestCase("0", "-0", 0)]
        [TestCase("0", "0e10", 0)]
        [TestCase("-2", "1", -1)]
        public void CompareTo(string s1, string s2, int expected)
        {
            var d1 = JsonDecimal.Parse(s1);
            var d2 = JsonDecimal.Parse(s2);

            Assert.That(d1.CompareTo((object) d2), Is.EqualTo(expected));
            Assert.That(d2.CompareTo((object) d1), Is.EqualTo(-expected));

            Assert.That(d1 == d2, Is.EqualTo(expected == 0));
            Assert.That(d1 > d2, Is.EqualTo(expected > 0));
            Assert.That(d1 >= d2, Is.EqualTo(expected >= 0));
            Assert.That(d1 < d2, Is.EqualTo(expected < 0));
            Assert.That(d1 <= d2, Is.EqualTo(expected <= 0));
        }

        [Test]
        public void CompareTo_Null_ReturnsOne()
        {
            Assert.That(new JsonDecimal().CompareTo(null), Is.EqualTo(1));
        }

        [Test]
        public void CompareTo_InvalidType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                int _ = JsonDecimal.Zero.CompareTo("");
            });
        }

        [TestCase("0", "0")]
        [TestCase("-0", "-0")]
        [TestCase("1000000000000000000000000000000000", "1000000000000000000000000000000000")]
        [TestCase("10000000000000000000000000000000000", "1E+34")]
        [TestCase("1234567890123456789012345678901234", "1234567890123456789012345678901234")]
        [TestCase("12345678901234567890123456789012340", "1.234567890123456789012345678901234E+34")]
        [TestCase("0.0000000000000000000000000000000001", "0.0000000000000000000000000000000001")]
        [TestCase("0.00000000000000000000000000000000001", "1E-35")]
        [TestCase("1.000000000000000000000000000000001", "1.000000000000000000000000000000001")]
        [TestCase("0.1000000000000000000000000000000001", "0.1000000000000000000000000000000001")]
        [TestCase("0.01000000000000000000000000000000001", "1.000000000000000000000000000000001E-2")]
        [TestCase("0.1234567890123456789012345678901234", "0.1234567890123456789012345678901234")]
        [TestCase("0.01234567890123456789012345678901234", "1.234567890123456789012345678901234E-2")]
        public void ToString_Default(string value, string expected)
        {
            Assert.That(JsonDecimal.Parse(value).ToString(), Is.EqualTo(expected));
        }

        [TestCase("0", ".1", "0.0")]
        public void ToString_Format(string value, string format, string expected)
        {
            Assert.That(JsonDecimal.Parse(value).ToString(format), Is.EqualTo(expected));
        }

        // Packing
        [TestCase("1E-6177")]
        [TestCase("123456789012345678901234567890123E-6176")]
        [TestCase("12345678901234567890123456789012345")]
        [TestCase("1234567890123456789012345678901234E+6112")]

        // Parsing
        [TestCase("10000000000000000000000000000000001")]
        [TestCase("12345678901234567890123456789012340.1")]
        [TestCase("1.0000000000000000000000000000000001")]
        [TestCase("1234567890123456789012345678901234.1")]
        [TestCase("1e12345")]
        public void Parse_Overflow(string s)
        {
            Assert.Throws<OverflowException>(() => JsonDecimal.Parse(s));
            Assert.That(JsonDecimal.TryParse(s, out _, out _), Is.False);
        }

        [TestCase("")]
        [TestCase("foo")]
        [TestCase("1e0000000000000000")]
        [TestCase("00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "000000")]
        [TestCase("0." +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "000000")]
        [TestCase("1" +
                  "00000000000000000000000000000000000000000000000000" +
                  "00000000000000000000000000000000000000000000000000" +
                  "000000000000000000000000000e0")]
        public void Parse_InvalidFormat(string s)
        {
            Assert.Throws<FormatException>(() => JsonDecimal.Parse(s));
            Assert.That(JsonDecimal.TryParse(s, out _, out _), Is.False);
        }

        [Test]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JsonDecimal.Parse(null));
            Assert.Throws<ArgumentNullException>(() => JsonDecimal.TryParse(null, out _, out _));
        }
    }
}
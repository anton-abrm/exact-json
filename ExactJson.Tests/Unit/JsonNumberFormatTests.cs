using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonNumberFormatTests
    {
        private static readonly string[] ValidFormats =
        {
            "-", "-2", "-.2", "-2.2",
            "", "2", ".2", "2.2",
            "e", "2e", ".2e", "2.2e",
            "e,4", "2e,4", ".2e,4", "2.2e,4",
            "e3", "2e3", ".2e3", "2.2e3",
            "e3,4", "2e3,4", ".2e3,4", "2.2e3,4",
            "E",
            "e+", "E+",
            "e,-1",
            "29", ".29", "29.29",
            "29e", ".29e", "29.29e",
            "e,49", "2e,49", ".2e,49", "2.2e,49",
            "e12", "2e12", ".2e12", "2.2e12",
            "e,-127",
            "255.255e15,127",
        };

        private static readonly string[] InvalidFormats =
        {
            " ",
            "0", "e0", "e,-128",
            "256", ".256", "e16", "e,128",
        };

        [TestCaseSource(nameof(ValidFormats))]
        public void RoundTrip(string format)
        {
            var result = JsonNumberFormat.Parse(format).ToString();

            Assert.That(result, Is.EqualTo(format));
        }

        [TestCaseSource(nameof(InvalidFormats))]
        public void Parse_InvalidFormats_ThrowsFormatException(string format)
        {
            Assert.Throws<FormatException>(() => JsonNumberFormat.Parse(format));
        }

        [Test]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JsonNumberFormat.Parse(null));
        }

        [TestCase(0)]
        [TestCase(256)]
        public void SetIntegralDigits_OutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => JsonNumberFormat.Decimal.SetIntegralDigits(value));
        }

        [TestCase(-1)]
        [TestCase(256)]
        public void SetFractionalDigits_OutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => JsonNumberFormat.Decimal.WithFractionalDigits(value));
        }

        [TestCase(0)]
        [TestCase(16)]
        public void SetExponentDigits_OutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => JsonNumberFormat.Exponential.WithExponentDigits(value));
        }

        [TestCase(-128)]
        [TestCase(128)]
        public void SetPointPosition_OutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => JsonNumberFormat.Exponential.WithPointPosition(value));
        }

        [TestCase("", "", true)]
        [TestCase("e", "e", true)]
        [TestCase("", "e", false)]
        public void Equality(string strX, string strY, bool expected)
        {
            var x = JsonNumberFormat.Parse(strX);
            var y = JsonNumberFormat.Parse(strY);

            Assert.That(x == y, Is.EqualTo(expected));
            Assert.That(x != y, Is.EqualTo(!expected));
        }

        [TestCase("", "")]
        [TestCase("e", "e")]
        public void HashCode(string strX, string strY)
        {
            int hashX = JsonNumberFormat.Parse(strX).GetHashCode();
            int hashY = JsonNumberFormat.Parse(strY).GetHashCode();

            Assert.That(hashX, Is.EqualTo(hashY));
        }

        [Test]
        public void Equals_DecimalsWithDifferentExponentialValues()
        {
            var x = JsonNumberFormat.Decimal;

            x = x.WithExponentDigits(1);
            x = x.WithPrintPlusExponentSign(false);
            x = x.WithUpperCaseExponentSign(false);
            x = x.WithPointPosition(1);

            var y = JsonNumberFormat.Decimal;

            y = y.WithExponentDigits(2);
            y = y.WithPrintPlusExponentSign(true);
            y = y.WithUpperCaseExponentSign(true);
            y = y.WithPointPosition(2);

            bool result = x.Equals((object) y);

            Assert.That(result, Is.True);
        }

        [TestCase((sbyte) 0, "")]
        public void For_Int8(sbyte value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase((byte) 0, "")]
        public void For_UInt8(byte value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase((short) 0, "")]
        public void For_Int16(short value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase((ushort) 0, "")]
        public void For_UInt16(ushort value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(0U, "")]
        public void For_UInt32(uint value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(0, "")]
        public void For_Int32(int value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(0UL, "")]
        public void For_UInt64(ulong value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(0L, "")]
        public void For_Int64(long value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase("0", "")]
        public void For_Decimal(string value, string expectedFormat)
        {
            decimal d = decimal.Parse(value);

            Assert.That(JsonNumberFormat.For(d).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(0.0, ".1")]
        [TestCase(1E-4, ".1")]
        [TestCase(1E+14, ".1")]
        [TestCase(1E-5, ".1E+")]
        [TestCase(1E+15, ".1E+")]
        public void For_Double(double value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }

        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void For_Double_OutOfRange_ThrowsArgumentOutOfRangeException(double value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonNumberFormat.For(value));
        }

        [TestCase(float.NaN)]
        [TestCase(float.NegativeInfinity)]
        [TestCase(float.PositiveInfinity)]
        public void For_Single_OutOfRange_ThrowsArgumentOutOfRangeException(float value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonNumberFormat.For(value));
        }

        [TestCase(0.0f, ".1")]
        [TestCase(1E-4f, ".1")]
        [TestCase(1E+6f, ".1")]
        [TestCase(1E-5f, ".1E+")]
        [TestCase(1E+7f, ".1E+")]
        public void For_Single(float value, string expectedFormat)
        {
            Assert.That(JsonNumberFormat.For(value).ToString(), Is.EqualTo(expectedFormat));
        }
    }
}
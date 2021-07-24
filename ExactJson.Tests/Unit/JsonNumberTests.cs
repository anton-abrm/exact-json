// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonNumberTests
    {
        #region Create

        [TestCase(byte.MinValue)]
        [TestCase(byte.MaxValue)]
        public void Create_UInt8(byte value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(sbyte.MinValue)]
        [TestCase(sbyte.MaxValue)]
        public void Create_Int8(sbyte value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(short.MinValue)]
        [TestCase(short.MaxValue)]
        public void Create_Int16(short value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(ushort.MinValue)]
        [TestCase(ushort.MaxValue)]
        public void Create_UInt16(ushort value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }


        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void Create_Int32(int value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        public void Create_UInt32(uint value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        public void Create_Int64Max(long value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        public void Create_UInt64Max(ulong value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(0.0)]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        [TestCase(double.Epsilon)]
        public void Create_Double(double value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase(0.0f)]
        [TestCase(float.MaxValue)]
        [TestCase(float.MinValue)]
        [TestCase(float.Epsilon)]
        public void Create_Single(float value)
        {
            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        [TestCase("0")]
        [TestCase("-79228162514264337593543950335")]
        [TestCase("79228162514264337593543950335")]
        public void Create_Decimal(string s)
        {
            decimal value = decimal.Parse(s);

            var num = JsonNumber.Create(value);

            Assert.That(num.Value, Is.EqualTo((JsonDecimal) value));
            Assert.That(num.Format, Is.EqualTo(JsonNumberFormat.For(value)));
        }

        #endregion

        #region NodeType

        [Test]
        public void NodeType_ByDefault_Number()
        {
            var num = JsonNumber.Create(int.MaxValue);

            Assert.That(num.NodeType, Is.EqualTo(JsonNodeType.Number));
        }

        #endregion

        #region ValueAsDecimal

        [Test]
        public void ValueAsDecimal_BeingInt32Number_ReturnsInt32Value()
        {
            var num = JsonNumber.Create(int.MaxValue);

            var result = num.Value;

            Assert.That(result, Is.EqualTo((JsonDecimal) int.MaxValue));
        }

        [Test]
        public void ValueAsDecimal_BeingInt64Number_ReturnsInt64Value()
        {
            var num = JsonNumber.Create(long.MaxValue);

            var result = num.Value;

            Assert.That(result, Is.EqualTo((JsonDecimal) long.MaxValue));
        }

        [Test]
        public void ValueAsDecimal_BeingDecimalNumber_ReturnsDecimalValue()
        {
            var num = JsonNumber.Create(decimal.MaxValue);

            var result = num.Value;

            Assert.That(result, Is.EqualTo((JsonDecimal) decimal.MaxValue));
        }

        #endregion

        #region Equals

        [Test]
        public void Equals_BeingInt32NumberAndGivenNull_ReturnsFalse()
        {
            object num = JsonNumber.Create(int.MaxValue);

            Assert.That(num.Equals(null), Is.False);
        }

        [Test]
        public void Equals_BeingInt64NumberAndGivenNull_ReturnsFalse()
        {
            object num = JsonNumber.Create(long.MaxValue);

            Assert.That(num.Equals(null), Is.False);
        }

        [Test]
        public void Equals_BeingDecimalNumberAndGivenNull_ReturnsFalse()
        {
            object num = JsonNumber.Create(decimal.MaxValue);

            Assert.That(num.Equals(null), Is.False);
        }

        [Test]
        public void Equals_BeingInt32NumberAndGivenEqualInt32Number_ReturnsTrue()
        {
            object num1 = JsonNumber.Create(int.MaxValue);
            object num2 = JsonNumber.Create(int.MaxValue);

            Assert.That(num1.Equals(num2), Is.True);
        }

        [Test]
        public void Equals_BeingInt64NumberAndGivenEqualInt64Number_ReturnsTrue()
        {
            object num1 = JsonNumber.Create(long.MaxValue);
            object num2 = JsonNumber.Create(long.MaxValue);

            Assert.That(num1.Equals(num2), Is.True);
        }

        [Test]
        public void Equals_BeingDecimalNumberAndGivenEqualDecimalNumber_ReturnsTrue()
        {
            object num1 = JsonNumber.Create(decimal.MaxValue);
            object num2 = JsonNumber.Create(decimal.MaxValue);

            Assert.That(num1.Equals(num2), Is.True);
        }

        [Test]
        public void Equals_BeingInt32NumberAndGivenNotEqualInt32Number_ReturnsFalse()
        {
            object num1 = JsonNumber.Create(int.MaxValue);
            object num2 = JsonNumber.Create(int.MinValue);

            Assert.That(num1.Equals(num2), Is.False);
        }

        [Test]
        public void Equals_BeingInt64NumberAndGivenNotEqualInt64Number_ReturnsFalse()
        {
            object num1 = JsonNumber.Create(long.MaxValue);
            object num2 = JsonNumber.Create(long.MinValue);

            Assert.That(num1.Equals(num2), Is.False);
        }

        [Test]
        public void Equals_BeingDecimalNumberAndGivenNotEqualDecimalNumber_ReturnsFalse()
        {
            object num1 = JsonNumber.Create(decimal.MaxValue);
            object num2 = JsonNumber.Create(decimal.MinValue);

            Assert.That(num1.Equals(num2), Is.False);
        }

        #endregion

        #region GetHashCode

        [Test]
        public void GetHashCode_TwoInt32NumbersEqual_HashCodesMatch()
        {
            int hash1 = JsonNumber.Create(int.MaxValue).GetHashCode();
            int hash2 = JsonNumber.Create(int.MaxValue).GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_TwoInt64NumbersEqual_HashCodesMatch()
        {
            int hash1 = JsonNumber.Create(long.MaxValue).GetHashCode();
            int hash2 = JsonNumber.Create(long.MaxValue).GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_TwoDecimalNumbersEqual_HashCodesMatch()
        {
            int hash1 = JsonNumber.Create(decimal.MaxValue).GetHashCode();
            int hash2 = JsonNumber.Create(decimal.MaxValue).GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        #endregion

        #region Write

        [Test]
        public void Write_BeingInt32NumberAndGivenNull_ThrowsArgumentNullException()
        {
            var num = JsonNumber.Create(int.MaxValue);

            Assert.Throws<ArgumentNullException>(() => num.WriteTo((JsonWriter) null));
        }

        [Test]
        public void Write_BeingInt64NumberAndGivenNull_ThrowsArgumentNullException()
        {
            var num = JsonNumber.Create(long.MaxValue);

            Assert.Throws<ArgumentNullException>(() => num.WriteTo((JsonWriter) null));
        }

        [Test]
        public void Write_BeingDecimalNumberAndGivenNull_ThrowsArgumentNullException()
        {
            var num = JsonNumber.Create(decimal.MaxValue);

            Assert.Throws<ArgumentNullException>(() => num.WriteTo((JsonWriter) null));
        }

        [Test]
        public void Write_BeingInt32Number_WritesDecimalNumber()
        {
            var mock = new Mock<JsonWriter>();

            var num = JsonNumber.Create(int.MaxValue);

            num.WriteTo(mock.Object);

            mock.Verify(w => w.WriteNumber(int.MaxValue, JsonNumberFormat.For(int.MaxValue)), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Write_BeingInt64Number_WritesDecimalNumber()
        {
            var mock = new Mock<JsonWriter>();

            var num = JsonNumber.Create(long.MaxValue);

            num.WriteTo(mock.Object);

            mock.Verify(w => w.WriteNumber(long.MaxValue, JsonNumberFormat.For(long.MaxValue)), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Write_BeingDecimalNumber_WritesDecimalNumber()
        {
            var mock = new Mock<JsonWriter>();

            var num = JsonNumber.Create(decimal.MaxValue);

            num.WriteTo(mock.Object);

            mock.Verify(w => w.WriteNumber(decimal.MaxValue, JsonNumberFormat.For(decimal.MaxValue)), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_Number_OutputFormattedNumber()
        {
            var result = JsonNumber.Create(1, ".1").ToString();

            Assert.That(result, Is.EqualTo("1.0"));
        }

        #endregion
    }
}
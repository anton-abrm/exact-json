using System;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    internal sealed class UInt128Tests
    {
        private static object[] ShiftRightInputs => new object[]
        {
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), -4,
                new UInt128(0x0000000000000000, 0x000000000000000D),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 0,
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 4,
                new UInt128(0x0DEADBEEFBAADF00, 0xDFEEDFACECAFEBEE),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 60,
                new UInt128(0x000000000000000D, 0xEADBEEFBAADF00DF),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 64,
                new UInt128(0x0000000000000000, 0xDEADBEEFBAADF00D),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 68,
                new UInt128(0x0000000000000000, 0x0DEADBEEFBAADF00),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 124,
                new UInt128(0x0000000000000000, 0x000000000000000D),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 128,
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 132,
                new UInt128(0x0DEADBEEFBAADF00, 0xDFEEDFACECAFEBEE),
            },
        };

        private static object[] ShiftLeftInputs => new object[]
        {
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), -4,
                new UInt128(0xF000000000000000, 0x0000000000000000),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 0,
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 4,
                new UInt128(0xEADBEEFBAADF00DF, 0xEEDFACECAFEBEEF0),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 60,
                new UInt128(0xDFEEDFACECAFEBEE, 0xF000000000000000),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 64,
                new UInt128(0xFEEDFACECAFEBEEF, 0x0000000000000000),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 68,
                new UInt128(0xEEDFACECAFEBEEF0, 0x0000000000000000),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 124,
                new UInt128(0xF000000000000000, 0x0000000000000000),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 128,
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF),
            },
            new object[]
            {
                new UInt128(0xDEADBEEFBAADF00D, 0xFEEDFACECAFEBEEF), 132,
                new UInt128(0xEADBEEFBAADF00DF, 0xEEDFACECAFEBEEF0),
            },
        };


        [TestCaseSource(nameof(ShiftRightInputs))]
        public void ShiftRight(UInt128 value, int shift, UInt128 expected)
        {
            var result = value >> shift;

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(ShiftLeftInputs))]
        public void ShiftLeft(UInt128 value, int shift, UInt128 expected)
        {
            var result = value << shift;

            Assert.That(result, Is.EqualTo(expected));
        }


        [TestCase(-1)]
        [TestCase(39)]
        public void Pow10_OutOfRangeExponent_ThrowsArgumentOutOfRangeException(int exponent)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => UInt128.Pow10(exponent));
        }

        [TestCase(-1)]
        [TestCase(40)]
        public void Normalize_OutOfRangeExponent_ThrowsArgumentOutOfRangeException(int digits)
        {
            var value = UInt128.Zero;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => UInt128.Normalize(ref value, digits));
        }

        [TestCase(-1)]
        [TestCase(40)]
        public void Denormalize_OutOfRangeExponent_ThrowsArgumentOutOfRangeException(int exponent)
        {
            var value = UInt128.Zero;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => UInt128.Denormalize(ref value, exponent));
        }

        [TestCase("0", 1, "0", 0)]
        [TestCase("0", 2, "0", 1)]
        [TestCase("1", 1, "1", 0)]
        [TestCase("1", 2, "10", 1)]
        [TestCase("1", 39, "100000000000000000000000000000000000000", 38)]
        [TestCase("12", 39, "120000000000000000000000000000000000000", 37)]
        [TestCase("1234567890", 20, "12345678900000000000", 10)]
        [TestCase("123456789012345678901234567890123456789", 39,
            "123456789012345678901234567890123456789", 0)]
        public void Normalize(string valueString,
                              int digits,
                              string normValueString,
                              int resultedExponent)
        {
            var value = UInt128.Parse(valueString);

            Assert.That(UInt128.Normalize(ref value, digits), Is.EqualTo(resultedExponent));
            Assert.That(value, Is.EqualTo(UInt128.Parse(normValueString)));
        }

        [TestCase("0", 0, "0", 0)]
        [TestCase("0", 1, "0", 1)]
        [TestCase("1", 1, "1", 0)]
        [TestCase("1", 38, "1", 0)]
        [TestCase("10", 1, "1", 1)]
        [TestCase("12345678900000000000", 10, "1234567890", 10)]
        [TestCase("100000000000000000000000000000000000000", 38, "1", 38)]
        [TestCase("120000000000000000000000000000000000000", 38, "12", 37)]
        [TestCase("123456789012345678901234567890123456789", 38,
            "123456789012345678901234567890123456789", 0)]
        public void Denormalize(string valueString,
                                int exponent,
                                string denValueString,
                                int resultedExponent)
        {
            var value = UInt128.Parse(valueString);

            Assert.That(UInt128.Denormalize(ref value, exponent), Is.EqualTo(resultedExponent));
            Assert.That(value, Is.EqualTo(UInt128.Parse(denValueString)));
        }

        [TestCase(0, "1")]
        [TestCase(1, "10")]
        [TestCase(38, "100000000000000000000000000000000000000")]
        public void Pow10(int power, string result)
        {
            Assert.That(UInt128.Pow10(power), Is.EqualTo(UInt128.Parse(result)));
        }

        [Test]
        public void DivRem_ZeroDenominator_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(()
                => UInt128.DivRem(UInt128.MaxValue, UInt128.Zero, out _));
        }

        [TestCase(1UL, 1UL, 1UL, 1UL, true)]
        [TestCase(2UL, 1UL, 1UL, 1UL, false)]
        [TestCase(1UL, 2UL, 1UL, 1UL, false)]
        [TestCase(1UL, 1UL, 2UL, 1UL, false)]
        [TestCase(1UL, 1UL, 1UL, 2UL, false)]
        public void Equals(ulong ah, ulong al, ulong bh, ulong bl, bool result)
        {
            var a = new UInt128(ah, al);
            var b = new UInt128(bh, bl);

            Assert.That(a.Equals(b), Is.EqualTo(result));
            Assert.That(a.Equals((object) b), Is.EqualTo(result));
            Assert.That(a == b, Is.EqualTo(result));
            Assert.That(a != b, Is.Not.EqualTo(result));
        }

        [TestCase("0", byte.MinValue)]
        [TestCase("255", byte.MaxValue)]
        public void CastToByte(string value, byte expected)
        {
            var result = (byte) UInt128.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("0", ushort.MinValue)]
        [TestCase("65535", ushort.MaxValue)]
        public void CastToUInt16(string value, ushort expected)
        {
            var result = (ushort) UInt128.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("0", uint.MinValue)]
        [TestCase("4294967295", uint.MaxValue)]
        public void CastToUInt32(string value, uint expected)
        {
            var result = (uint) UInt128.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("0", ulong.MinValue)]
        [TestCase("18446744073709551615", ulong.MaxValue)]
        public void CastToUInt64(string value, ulong expected)
        {
            var result = (ulong) UInt128.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0UL, 256UL)]
        [TestCase(1UL, 0UL)]
        public void CastToByte_OutOfRange_ThrowsOverflowException(ulong hi, ulong lo)
        {
            var value = new UInt128(hi, lo);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (byte) value;
            });
        }

        [TestCase(0UL, 65536UL)]
        [TestCase(1UL, 0UL)]
        public void CastToUInt16_OutOfRange_ThrowsOverflowException(ulong hi, ulong lo)
        {
            var value = new UInt128(hi, lo);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (ushort) value;
            });
        }

        [TestCase(0UL, 4294967296UL)]
        [TestCase(1UL, 0UL)]
        public void CastToUInt32_OutOfRange_ThrowsOverflowException(ulong hi, ulong lo)
        {
            var value = new UInt128(hi, lo);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (uint) value;
            });
        }

        [TestCase(1UL, 0UL)]
        public void CastToUInt64_OutOfRange_ThrowsOverflowException(ulong hi, ulong lo)
        {
            var value = new UInt128(hi, lo);

            Assert.Throws<OverflowException>(() =>
            {
                var _ = (ulong) value;
            });
        }

        [TestCase("")]
        [TestCase("q")]
        public void Parse_InvalidFormat_ThrowsFormatException(string source)
        {
            Assert.Throws<FormatException>(() =>
            {
                UInt128.Parse(source);
            });
        }

        [TestCase("340282366920938463463374607431768211456")]
        [TestCase("1234567890123456789012345678901234567890")]
        public void Parse_OutOfRange_ThrowsOverflowException(string source)
        {
            Assert.Throws<OverflowException>(() =>
            {
                UInt128.Parse(source);
            });
        }

        [TestCase("0")]
        [TestCase("1")]
        [TestCase("12")]
        [TestCase("1234")]
        [TestCase("12345678")]
        [TestCase("1234567812345678")]
        [TestCase("12345678123456781234567812345678")]
        [TestCase("340282366920938463463374607431768211455")]
        public void RoundTrip(string source)
        {
            var result = UInt128.Parse(source).ToString();

            Assert.That(result, Is.EqualTo(source));
        }

        [TestCase("0", "0")]
        [TestCase("340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
        public void GetHashCode(string x, string y)
        {
            int hashX = UInt128.Parse(x).GetHashCode();
            int hashY = UInt128.Parse(y).GetHashCode();

            Assert.That(hashX, Is.EqualTo(hashY));
        }

        [TestCase(0UL, 0UL, 0UL, 0UL, 0)]
        [TestCase(0UL, 1UL, 1UL, 0UL, -1)]
        [TestCase(0UL, 1UL, 0UL, 0UL, 1)]
        public void CompareTo(ulong ah, ulong al, ulong bh, ulong bl, int expected)
        {
            var a = new UInt128(ah, al);
            var b = new UInt128(bh, bl);

            Assert.That(a.CompareTo(b), Is.EqualTo(expected));
            Assert.That(b.CompareTo(a), Is.EqualTo(-expected));

            Assert.That(a < b, Is.EqualTo(expected < 0));
            Assert.That(a <= b, Is.EqualTo(expected <= 0));
            Assert.That(a > b, Is.EqualTo(expected > 0));
            Assert.That(a >= b, Is.EqualTo(expected >= 0));
        }

        [TestCase(0UL, 0UL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL)]
        public void OnesComplement(ulong ah, ulong al, ulong bh, ulong bl)
        {
            var a = new UInt128(ah, al);
            var b = new UInt128(bh, bl);

            Assert.That(~a, Is.EqualTo(b));
        }

        [TestCase(0UL, 0UL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0UL, 0UL)]
        public void BitwiseAnd(ulong ah, ulong al, ulong bh, ulong bl, ulong ch, ulong cl)
        {
            var a = new UInt128(ah, al);
            var b = new UInt128(bh, bl);
            var c = new UInt128(ch, cl);

            Assert.That(a & b, Is.EqualTo(c));
        }

        [TestCase(0UL, 0UL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL)]
        public void BitwiseOr(ulong ah, ulong al, ulong bh, ulong bl, ulong ch, ulong cl)
        {
            var a = new UInt128(ah, al);
            var b = new UInt128(bh, bl);
            var c = new UInt128(ch, cl);

            Assert.That(a | b, Is.EqualTo(c));
        }

        [Test]
        public void Constructor_4Uint32()
        {
            var a = new UInt128(0x1, 0x2, 0x3, 0x4);
            var b = new UInt128(0x0000000100000002UL, 0x0000000300000004UL);

            Assert.That(a, Is.EqualTo(b));
        }

        private static readonly object[] DivRemChallenges =
        {
            new object[] // Overflow Q
            {
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
            },
            new object[] // Overflow Mul
            {
                new UInt128(0xFFFFFFFF, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFF, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFE, 0x00000003),
            },
            new object[] // Product greater than remainder 
            {
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0xFFFFFFFF),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x80000000),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x7FFFFFFE),
            },
            new object[] // Optimization
            {
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
            },
            new object[] // Optimization
            {
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
            },
            new object[] // Optimization
            {
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000001, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
            },
            new object[] // Optimization
            {
                new UInt128(0x00000001, 0x00000000, 0x00000000, 0x00000000),
                new UInt128(0x00000001, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
                new UInt128(0x00000001, 0x00000000, 0x00000000, 0x00000000),
            },
            new object[] // CountBits
            {
                new UInt128(0x80000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x80000000, 0x00000000, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x80000000, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x80000000, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x00008000, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00008000, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x00000080, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000080, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x00000008, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000008, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x00000002, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000002, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
            new object[] // CountBits
            {
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000001, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000001),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFE),
            },
        };

        [TestCaseSource(nameof(DivRemChallenges))]
        public void DivRem(UInt128 x, UInt128 y, UInt128 q, UInt128 r)
        {
            Assert.That(x / y, Is.EqualTo(q));
            Assert.That(x % y, Is.EqualTo(r));
        }

        private static readonly object[] MultiplyChallenges =
        {
            new object[]
            {
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF),
                new UInt128(0x00000001, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF),
                new UInt128(0xFFFFFFFE, 0xFFFFFFFE, 0x00000000, 0x00000001),
            },
            new object[] // Optimization
            {
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF),
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFE, 0x00000001),
            },
            new object[] // Optimization
            {
                new UInt128(0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0x00000000),
            },
            new object[] // Overflow
            {
                new UInt128(0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF),
                new UInt128(0x00000001, 0x00000001, 0x00000001, 0xFFFFFFFE),
                new UInt128(0x00000000, 0x00000000, 0xFFFFFFFC, 0x00000002),
            },
        };

        [TestCaseSource(nameof(MultiplyChallenges))]
        public void Multiply(UInt128 x, UInt128 y, UInt128 p)
        {
            Assert.That(x * y, Is.EqualTo(p));
        }

        [Test]
        public void Indexer_Get()
        {
            var value = new UInt128(1, 2, 3, 4);
            
            Assert.That(value[0], Is.EqualTo(4));
            Assert.That(value[1], Is.EqualTo(3));
            Assert.That(value[2], Is.EqualTo(2));
            Assert.That(value[3], Is.EqualTo(1));
        }
        
        [TestCase(-1)]
        [TestCase(+4)]
        public void Indexer_Get_OutOfRange_ThrowsArgumentOutOfRangeException(int index)
        {
            var value = new UInt128();

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = value[index];
            });


        }
    }
}
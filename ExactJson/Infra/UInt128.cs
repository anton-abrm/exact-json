using System;
using System.Globalization;

namespace ExactJson.Infra
{
    internal readonly struct UInt128 : IComparable<UInt128>, IEquatable<UInt128>
    {
        private const int MinDigits = 1;
        private const int MaxDigits = 39;
        private const int MinExponent = MinDigits - 1;
        private const int MaxExponent = MaxDigits - 1;

        private static readonly UInt128[] Powers =
        {
            /* 00 */ new UInt128(0x0, 0x1),
            /* 01 */ new UInt128(0x0, 0x0A),
            /* 02 */ new UInt128(0x0, 0x64),
            /* 03 */ new UInt128(0x0, 0x3E8),
            /* 04 */ new UInt128(0x0, 0x2710),
            /* 05 */ new UInt128(0x0, 0x186A0),
            /* 06 */ new UInt128(0x0, 0x0F4240),
            /* 07 */ new UInt128(0x0, 0x0989680),
            /* 08 */ new UInt128(0x0, 0x5F5E100),
            /* 09 */ new UInt128(0x0, 0x3B9ACA00),
            /* 10 */ new UInt128(0x0, 0x2540BE400),
            /* 11 */ new UInt128(0x0, 0x174876E800),
            /* 12 */ new UInt128(0x0, 0x0E8D4A51000),
            /* 13 */ new UInt128(0x0, 0x09184E72A000),
            /* 14 */ new UInt128(0x0, 0x5AF3107A4000),
            /* 15 */ new UInt128(0x0, 0x38D7EA4C68000),
            /* 16 */ new UInt128(0x0, 0x2386F26FC10000),
            /* 17 */ new UInt128(0x0, 0x16345785D8A0000),
            /* 18 */ new UInt128(0x0, 0x0DE0B6B3A7640000),
            /* 19 */ new UInt128(0x0, 0x08AC7230489E80000),
            /* 20 */ new UInt128(0x5, 0x6BC75E2D63100000),
            /* 21 */ new UInt128(0x36, 0x35C9ADC5DEA00000),
            /* 22 */ new UInt128(0x21E, 0x19E0C9BAB2400000),
            /* 23 */ new UInt128(0x152D, 0x2C7E14AF6800000),
            /* 24 */ new UInt128(0x0D3C2, 0x1BCECCEDA1000000),
            /* 25 */ new UInt128(0x084595, 0x161401484A000000),
            /* 26 */ new UInt128(0x52B7D2, 0x0DCC80CD2E4000000),
            /* 27 */ new UInt128(0x33B2E3C, 0x09FD0803CE8000000),
            /* 28 */ new UInt128(0x204FCE5E, 0x3E25026110000000),
            /* 29 */ new UInt128(0x1431E0FAE, 0x6D7217CAA0000000),
            /* 30 */ new UInt128(0x0C9F2C9CD0, 0x4674EDEA40000000),
            /* 31 */ new UInt128(0x7E37BE2022, 0x0C0914B2680000000),
            /* 32 */ new UInt128(0x4EE2D6D415B, 0x085ACEF8100000000),
            /* 33 */ new UInt128(0x314DC6448D93, 0x38C15B0A00000000),
            /* 34 */ new UInt128(0x1ED09BEAD87C0, 0x378D8E6400000000),
            /* 35 */ new UInt128(0x13426172C74D82, 0x2B878FE800000000),
            /* 36 */ new UInt128(0x0C097CE7BC90715, 0x0B34B9F1000000000),
            /* 37 */ new UInt128(0x785EE10D5DA46D9, 0x0F436A000000000),
            /* 38 */ new UInt128(0x4B3B4CA85A86C47A, 0x098A224000000000),
        };

        public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
        public static readonly UInt128 Zero = new UInt128();

        private readonly ulong _hi;
        private readonly ulong _lo;
        
        public uint this[int index]
        {
            get
            {
                switch (index) {
                    case 3: return (uint) (_hi >> 32);
                    case 2: return (uint) (_hi & uint.MaxValue);
                    case 1: return (uint) (_lo >> 32);
                    case 0: return (uint) (_lo & uint.MaxValue);
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #region Constructors

        public UInt128(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        public UInt128(uint hh, uint hl, uint lh, uint ll)
        {
            _hi = ((ulong) hh << 32) | hl;
            _lo = ((ulong) lh << 32) | ll;
        }

        #endregion

        #region Equality

        public override bool Equals(object obj)
        {
            return obj is UInt128 other && Equals(other);
        }

        public bool Equals(UInt128 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = hash * 23 + _hi.GetHashCode();
                hash = hash * 23 + _lo.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(in UInt128 a, in UInt128 b)
        {
            return a._hi == b._hi &&
                   a._lo == b._lo;
        }

        public static bool operator !=(in UInt128 a, in UInt128 b)
        {
            return !(a == b);
        }

        #endregion

        #region Comparision

        public int CompareTo(UInt128 other)
        {
            int result = _hi.CompareTo(other._hi);

            if (result != 0) {
                return result;
            }

            return _lo.CompareTo(other._lo);
        }

        public static bool operator <(in UInt128 a, in UInt128 b)
        {
            return a._hi < b._hi || a._hi == b._hi && a._lo < b._lo;
        }

        public static bool operator >(in UInt128 a, in UInt128 b)
        {
            return a._hi > b._hi || a._hi == b._hi && a._lo > b._lo;
        }

        public static bool operator <=(in UInt128 a, in UInt128 b)
        {
            return a._hi < b._hi || a._hi == b._hi && a._lo <= b._lo;
        }

        public static bool operator >=(in UInt128 a, in UInt128 b)
        {
            return a._hi > b._hi || a._hi == b._hi && a._lo >= b._lo;
        }

        #endregion

        #region Casting

        public static explicit operator byte(in UInt128 value)
        {
            if (value._hi != 0) {
                throw new OverflowException();
            }

            return checked((byte) value._lo);
        }

        public static explicit operator ushort(in UInt128 value)
        {
            if (value._hi != 0) {
                throw new OverflowException();
            }

            return checked((ushort) value._lo);
        }

        public static explicit operator uint(in UInt128 value)
        {
            if (value._hi != 0) {
                throw new OverflowException();
            }

            return checked((uint) value._lo);
        }

        public static explicit operator ulong(in UInt128 value)
        {
            if (value._hi != 0) {
                throw new OverflowException();
            }

            return value._lo;
        }

        public static implicit operator UInt128(byte value)
        {
            return new UInt128(0, value);
        }

        public static implicit operator UInt128(ushort value)
        {
            return new UInt128(0, value);
        }

        public static implicit operator UInt128(uint value)
        {
            return new UInt128(0, value);
        }

        public static implicit operator UInt128(ulong value)
        {
            return new UInt128(0, value);
        }

        #endregion

        #region Bitwise

        public static UInt128 operator ~(in UInt128 a)
        {
            return new UInt128(~a._hi, ~a._lo);
        }

        public static UInt128 operator |(in UInt128 a, in UInt128 b)
        {
            return new UInt128(
                a._hi | b._hi,
                a._lo | b._lo);
        }

        public static UInt128 operator &(in UInt128 a, in UInt128 b)
        {
            return new UInt128(
                a._hi & b._hi,
                a._lo & b._lo);
        }

        public static UInt128 operator <<(in UInt128 a, int shift)
        {
            shift = (int) ((uint) shift % 128);

            if (shift == 0) {
                return a;
            }

            return shift < 64
                ? new UInt128((a._hi << shift) | (a._lo >> (64 - shift)), a._lo << shift)
                : new UInt128(a._lo << (shift - 64), 0);
        }

        public static UInt128 operator >>(in UInt128 a, int shift)
        {
            shift = (int) ((uint) shift % 128);

            if (shift == 0) {
                return a;
            }

            return shift < 64
                ? new UInt128(a._hi >> shift, (a._lo >> shift) | (a._hi << (64 - shift)))
                : new UInt128(0, a._hi >> (shift - 64));
        }

        #endregion

        #region Arithmetic

        private static UInt128 Multiply32(in UInt128 x, uint y, out uint overflow)
        {
            if (y == 0) {
                overflow = 0;
                return Zero;
            }

            if (x._hi == 0 && x._lo <= uint.MaxValue) {
                overflow = 0;
                return x._lo * y;
            }

            ulong hi = 0;
            ulong lo = 0;
            ulong ov = 0;

            for (var i = 3; i >= 0; i--) {
                ov = (ov << 32) | (hi >> 32);
                hi = (hi << 32) | (lo >> 32);
                lo = lo << 32;

                ulong prod = (ulong) y * x[i];

                unchecked {
                    lo += prod;
                }

                if (lo < prod) {
                    unchecked {
                        hi++;
                    }

                    if (hi < 1) {
                        ov++;
                    }
                }
            }

            overflow = (uint) ov;

            return new UInt128(hi, lo);
        }

        private static UInt128 DivRem32(in UInt128 num, uint den, out uint rem)
        {
            if (num._hi == 0) {
                rem = (uint) (num._lo % den);
                return num._lo / den;
            }

            var rem64 = 0UL;

            var hi = 0UL;
            var lo = 0UL;

            for (var i = 3; i >= 0; i--) {
                rem64 = (rem64 << 32) | num[i];
                hi = (hi << 32) | (lo >> 32);
                lo = (lo << 32) | (rem64 / den);
                rem64 %= den;
            }

            rem = (uint) rem64;

            return new UInt128(hi, lo);
        }

        public static UInt128 DivRem(in UInt128 num, in UInt128 den, out UInt128 rem)
        {
            if (den == Zero) {
                throw new DivideByZeroException();
            }

            if (num < den) {
                rem = num;
                return Zero;
            }

            UInt128 qtn;

            // Optimization
            if (den._hi == 0) {
                if (num._hi == 0) {
                    qtn = num._lo / den._lo;
                    rem = num._lo % den._lo;

                    return qtn;
                }

                if (den._lo <= uint.MaxValue) {
                    qtn = DivRem32(num, (uint) den._lo, out uint rem32);
                    rem = rem32;

                    return qtn;
                }
            }

            qtn = Zero;
            rem = Zero;

            int bits = CountBits(den);

            // Denominator Normalization
            var nDen = bits > 96
                ? den >> (bits - 96)
                : den << (96 - bits);

            for (var i = 3; i >= 0; i--) {
                rem = (rem << 32) | num[i];

                if (rem < den) {
                    qtn <<= 32;
                    continue;
                }

                // Optimization
                if (rem._hi == 0 && den._hi == 0) {
                    qtn = (qtn << 32) | (rem._lo / den._lo);
                    rem = rem._lo % den._lo;
                    continue;
                }

                // Remainder Normalization
                var nRem = bits > 96
                    ? rem >> (bits - 96)
                    : rem << (96 - bits);

                var q = (uint) Math.Min(nRem._hi / nDen._hi, uint.MaxValue);

                while (true) {
                    var prod = Multiply32(den, q, out uint overflow);

                    if (overflow == 0 && prod <= rem) {
                        rem -= prod;
                        break;
                    }

                    q--;
                }

                qtn = (qtn << 32) | q;
            }

            return qtn;
        }

        public static UInt128 operator +(in UInt128 x, in UInt128 y)
        {
            unchecked {
                ulong hi = x._hi + y._hi;
                ulong lo = x._lo + y._lo;

                if (lo < x._lo) {
                    hi++;
                }

                return new UInt128(hi, lo);
            }
        }

        public static UInt128 operator -(in UInt128 x, in UInt128 y)
        {
            unchecked {
                ulong hi = x._hi - y._hi;
                ulong lo = x._lo - y._lo;

                if (lo > x._lo) {
                    hi--;
                }

                return new UInt128(hi, lo);
            }
        }

        public static UInt128 operator *(in UInt128 x, in UInt128 y)
        {
            if (x._hi == 0 && x._lo <= uint.MaxValue) {
                return Multiply32(y, (uint) x._lo, out uint _);
            }

            if (y._hi == 0 && y._lo <= uint.MaxValue) {
                return Multiply32(x, (uint) y._lo, out uint _);
            }

            var prod = Zero;

            for (var i = 3; i >= 0; i--) {
                prod <<= 32;

                uint w = y[i];
                var p = Zero;

                for (int j = 3 - i; j >= 0; j--) {
                    p <<= 32;
                    p += (ulong) w * x[j];
                }

                prod += p;
            }

            return prod;
        }

        public static UInt128 operator /(in UInt128 x, in UInt128 y)
        {
            return DivRem(x, y, out _);
        }

        public static UInt128 operator %(in UInt128 x, in UInt128 y)
        {
            DivRem(x, y, out var remainder);
            return remainder;
        }

        #endregion

        public static UInt128 Parse(string value)
        {
            UInt128 result = 0;

            if (value.Length == 0) {
                throw new FormatException();
            }

            foreach (char ch in value) {
                if (ch < '0' || ch > '9') {
                    throw new FormatException();
                }

                result = Multiply32(result, 10, out uint overflow);

                if (overflow != 0) {
                    throw new OverflowException();
                }

                var digit = (UInt128) (ch - '0');

                result += digit;

                if (result < digit) {
                    throw new OverflowException();
                }
            }

            return result;
        }

        public override string ToString()
        {
            if (_hi == 0) {
                return _lo.ToString(CultureInfo.InvariantCulture);
            }

            var quot = this;

            var sb = StringBuilderCache.Acquire(MaxDigits);

            while (quot != Zero) {
                quot = DivRem32(quot, 10, out uint rem);
                sb.Append((char) (rem + '0'));
            }

            StringBuilderUtil.Reverse(sb, 0, sb.Length);

            return StringBuilderCache.GetStringAndRelease(sb);
        }


        private static int CountBits(in UInt128 value)
        {
            var count = 0;
            ulong val64 = value._lo;

            if (value._hi != 0) {
                val64 = value._hi;
                count += 64;
            }

            if (val64 >= 1UL << 63) {
                count += 64;
                return count;
            }

            if (val64 >= 1UL << 31) {
                val64 >>= 32;
                count += 32;
            }

            if (val64 >= 1UL << 15) {
                count += 16;
                val64 >>= 16;
            }

            if (val64 >= 1UL << 7) {
                count += 8;
                val64 >>= 8;
            }

            if (val64 >= 1UL << 3) {
                count += 4;
                val64 >>= 4;
            }

            if (val64 >= 1UL << 1) {
                count += 2;
                val64 >>= 2;
            }

            if (val64 >= 1UL) {
                count += 1;
            }

            return count;
        }

        public static UInt128 Pow10(int exponent)
        {
            if (exponent < MinExponent || exponent > MaxExponent) {
                throw new ArgumentOutOfRangeException(nameof(exponent));
            }

            return Powers[exponent];
        }

        public static int Normalize(ref UInt128 value, int digits)
        {
            if (digits < MinDigits || digits > MaxDigits) {
                throw new ArgumentOutOfRangeException(nameof(digits));
            }

            if (value == Zero) {
                return digits - 1;
            }

            var exponent = 0;

            for (var e = 32; e > 0; e /= 2) {
                if (digits >= e && value < Powers[digits - e]) {
                    value *= Powers[e];
                    exponent += e;
                }
            }

            return exponent;
        }

        public static int Denormalize(ref UInt128 value, int exponent)
        {
            if (exponent < MinDigits - 1 || exponent > MaxDigits - 1) {
                throw new ArgumentOutOfRangeException(nameof(exponent));
            }

            if (value == Zero) {
                return exponent;
            }

            int remaining = exponent;

            for (var e = 32; e > 0; e /= 2) {
                if (remaining >= e && value >= Powers[e]) {
                    var tmp = DivRem(value, Powers[e], out var rem);

                    if (rem == Zero) {
                        value = tmp;
                        remaining -= e;
                    }
                }
            }

            return exponent - remaining;
        }
    }
}
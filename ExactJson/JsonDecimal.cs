using System;
using System.Globalization;
using System.Text;

using ExactJson.Infra;

namespace ExactJson
{
    public readonly struct JsonDecimal :
        IComparable<JsonDecimal>,
        IComparable,
        IEquatable<JsonDecimal>,
        IFormattable
    {
        private const int MinExponent = -6176;
        private const int MaxExponent = +6111;

        public static readonly JsonDecimal Zero = new JsonDecimal();

        // 9 999 999 999 999 999 999 999 999 999 999 999
        private static readonly UInt128 MaxCoefficient = new UInt128(0x0001ED09_BEAD87C0, 0x378D8E63_FFFFFFFF);

        private static readonly UInt128 SignMask = new UInt128(0x8000000000000000, 0x0);
        private static readonly UInt128 ShiftMask = new UInt128(0x6000000000000000, 0x0);
        private static readonly UInt128 ExponentMask14 = 0x3FFF;
        private static readonly UInt128 CoefficientMask111 = new UInt128(0x07FFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);
        private static readonly UInt128 CoefficientMask113 = new UInt128(0x1FFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);
        private static readonly UInt128 CoefficientFlag114 = new UInt128(0x2000000000000, 0x0);

        private const int MaxSignificandDigits = 34;
        private const int MaxSignificandScale = 33;

        private readonly UInt128 _bits;

        private static readonly JsonNumberFormat ExponentialFormat =
            JsonNumberFormat.Exponential
                            .WithUpperCaseExponentSign(true)
                            .WithPrintPlusExponentSign(true)
                            .WithPrintMinusZero(true);

        private static readonly JsonNumberFormat DecimalFormat =
            JsonNumberFormat.Decimal
                            .WithUpperCaseExponentSign(true)
                            .WithPrintPlusExponentSign(true)
                            .WithPrintMinusZero(true);

        #region Constructors

        private JsonDecimal(UInt128 bits)
        {
            _bits = bits;
        }

        #endregion

        #region Helpers

        private static JsonDecimal PackNumber(bool negative, UInt128 coefficient, int exponent)
        {
            PackNumber(negative, coefficient, exponent, out var result, true);
            return result;
        }

        private static bool PackNumber(bool negative, UInt128 coefficient, int exponent, out JsonDecimal num, bool throwOnError)
        {
            num = default;

            if (exponent < MinExponent || coefficient > MaxCoefficient) {
                goto overflowError;
            }

            exponent -= UInt128.Normalize(ref coefficient, MaxSignificandDigits);

            if (exponent < MinExponent || exponent > MaxExponent) {
                goto overflowError;
            }

            exponent -= MinExponent;

            var bits = UInt128.Zero;

            if (negative) {
                bits |= SignMask;
            }

            bits |= coefficient >> 110 <= 7
                ? ((UInt128) exponent << 113) | (coefficient & CoefficientMask113)
                : ((UInt128) exponent << 111) | (coefficient & CoefficientMask111) | ShiftMask;

            num = new JsonDecimal(bits);

            return true;

        overflowError:

            if (throwOnError) {
                throw new OverflowException();
            }

            return false;
        }

        #endregion

        #region Properties

        private static bool IsShifted(in JsonDecimal value)
        {
            return (value._bits & ShiftMask) == ShiftMask;
        }

        internal static bool IsNegative(in JsonDecimal value)
        {
            return (value._bits & SignMask) == SignMask;
        }

        internal static int GetScale(in JsonDecimal value)
        {
            return -GetExponent(value);
        }

        internal static int GetExponent(in JsonDecimal value)
        {
            return IsShifted(value)
                ? (int) ((value._bits >> 111) & ExponentMask14) + MinExponent
                : (int) ((value._bits >> 113) & ExponentMask14) + MinExponent;
        }

        internal static UInt128 GetCoefficient(in JsonDecimal value)
        {
            return IsShifted(value)
                ? (value._bits & CoefficientMask111) | CoefficientFlag114
                : value._bits & CoefficientMask113;
        }

        #endregion

        #region Comparision

        public int CompareTo(JsonDecimal that)
        {
            var coefX = GetCoefficient(this);
            var coefY = GetCoefficient(that);

            var expX = 0;
            var expY = 0;

            if (coefX != UInt128.Zero || coefY != UInt128.Zero) {
                expX = GetExponent(this);
                expY = GetExponent(that);
            }

            bool isNegativeX = coefX != UInt128.Zero && IsNegative(this);
            bool isNegativeY = coefY != UInt128.Zero && IsNegative(that);

            if (isNegativeX != isNegativeY) {
                return isNegativeY.CompareTo(isNegativeX);
            }

            if (expX != expY) {
                return isNegativeX
                    ? expY.CompareTo(expX)
                    : expX.CompareTo(expY);
            }

            return isNegativeX
                ? coefY.CompareTo(coefX)
                : coefX.CompareTo(coefY);
        }

        public int CompareTo(object obj)
        {
            switch (obj) {
                
                case null:
                    return 1;

                case JsonDecimal dec:
                    return CompareTo(dec);

                default:
                    throw new ArgumentException($"Object must be of type {nameof(JsonDecimal)}.", nameof(obj));
            }
        }

        public static bool operator <(JsonDecimal left, JsonDecimal right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(JsonDecimal left, JsonDecimal right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(JsonDecimal left, JsonDecimal right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(JsonDecimal left, JsonDecimal right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region Equality

        private static UInt128 GetEquatableBits(in JsonDecimal value)
        {
            return GetCoefficient(value) == UInt128.Zero ? UInt128.Zero : value._bits;
        }

        public bool Equals(JsonDecimal other)
        {
            return GetEquatableBits(this) == GetEquatableBits(other);
        }

        public override bool Equals(object obj)
        {
            return obj is JsonDecimal dec && Equals(dec);
        }

        public override int GetHashCode()
        {
            return GetEquatableBits(this).GetHashCode();
        }

        public static bool operator ==(JsonDecimal left, JsonDecimal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(JsonDecimal left, JsonDecimal right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Casting

        public static implicit operator JsonDecimal(byte value)
        {
            return PackNumber(false, value, 0);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonDecimal(sbyte value)
        {
            return PackNumber(value < 0, (ushort) Math.Abs((short) value), 0);
        }

        public static implicit operator JsonDecimal(short value)
        {
            return PackNumber(value < 0, (uint) Math.Abs((int) value), 0);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonDecimal(ushort value)
        {
            return PackNumber(false, value, 0);
        }

        public static implicit operator JsonDecimal(int value)
        {
            return PackNumber(value < 0, (ulong) Math.Abs((long) value), 0);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonDecimal(uint value)
        {
            return PackNumber(false, value, 0);
        }

        public static implicit operator JsonDecimal(long value)
        {
            return value == long.MinValue
                ? PackNumber(true, 0x8000000000000000, 0)
                : PackNumber(value < 0, (ulong) Math.Abs(value), 0);
        }

        [CLSCompliant(false)]
        public static implicit operator JsonDecimal(ulong value)
        {
            return PackNumber(false, value, 0);
        }

        public static implicit operator JsonDecimal(decimal value)
        {
            // TODO: Improve performance
            int[] bits = decimal.GetBits(value);

            var coefficient = new UInt128(0,
                (uint) bits[2],
                (uint) bits[1],
                (uint) bits[0]);

            var combination = (uint) bits[3];

            int exponent = -(int) ((combination >> 16) & 0xFF);
            bool negative = (combination & 0x80000000) != 0;

            return PackNumber(negative, coefficient, exponent);
        }

        public static explicit operator JsonDecimal(float value)
        {
            if (value == 0.0f) {
                return 0;
            }

            if (float.IsNaN(value) || float.IsInfinity(value)) {
                throw new OverflowException();
            }

            // TODO: Improve performance
            return Parse(value.ToString("R", CultureInfo.InvariantCulture));
        }

        public static explicit operator JsonDecimal(double value)
        {
            if (value == 0.0) {
                return 0;
            }

            if (double.IsNaN(value) || double.IsInfinity(value)) {
                throw new OverflowException();
            }

            // TODO: Improve performance
            return Parse(value.ToString("R", CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(JsonDecimal value)
        {
            return checked((sbyte) (long) value);
        }

        public static explicit operator byte(JsonDecimal value)
        {
            return checked((byte) (ulong) value);
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(JsonDecimal value)
        {
            return checked((ushort) (ulong) value);
        }

        public static explicit operator short(JsonDecimal value)
        {
            return checked((short) (long) value);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(JsonDecimal value)
        {
            return checked((uint) (ulong) value);
        }

        public static explicit operator int(JsonDecimal value)
        {
            return checked((int) (long) value);
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(JsonDecimal value)
        {
            var coefficient = GetCoefficient(value);

            if (coefficient == UInt128.Zero) {
                return 0UL;
            }

            if (IsNegative(value)) {
                throw new OverflowException();
            }

            int scale = GetScale(value);

            if (scale < MaxSignificandScale - 19 || scale > MaxSignificandScale) {
                throw new OverflowException();
            }

            var quotient = UInt128.DivRem(coefficient, UInt128.Pow10(scale), out var remainder);

            if (remainder != UInt128.Zero || quotient > ulong.MaxValue) {
                throw new OverflowException();
            }

            return (ulong) quotient;
        }

        public static explicit operator long(JsonDecimal value)
        {
            var coefficient = GetCoefficient(value);

            if (coefficient == UInt128.Zero) {
                return 0L;
            }

            int scale = GetScale(value);

            if (scale < MaxSignificandScale - 19 || scale > MaxSignificandScale) {
                throw new OverflowException();
            }

            var quotient = UInt128.DivRem(coefficient, UInt128.Pow10(scale), out var remainder);

            if (remainder != UInt128.Zero) {
                throw new OverflowException();
            }

            if (IsNegative(value)) {
                if (quotient > 9223372036854775808UL) {
                    throw new OverflowException();
                }

                if (quotient == 9223372036854775808UL) {
                    return long.MinValue;
                }

                return -(long) (ulong) quotient;
            }

            if (quotient > 9223372036854775807UL) {
                throw new OverflowException();
            }

            return (long) (ulong) quotient;
        }

        public static explicit operator float(JsonDecimal value)
        {
            return float.Parse(value.ToString(JsonNumberFormat.Exponential), CultureInfo.InvariantCulture);
        }

        public static explicit operator double(JsonDecimal value)
        {
            return double.Parse(value.ToString(JsonNumberFormat.Exponential), CultureInfo.InvariantCulture);
        }

        public static explicit operator decimal(JsonDecimal value)
        {
            var coefficient = GetCoefficient(value);

            if (coefficient == UInt128.Zero) {
                return new decimal(0, 0, 0, IsNegative(value), 0);
            }

            int scale = GetScale(value);

            if (scale < 5 || scale > 61) {
                throw new OverflowException();
            }

            scale -= UInt128.Denormalize(ref coefficient, Math.Min(scale, MaxSignificandScale));

            if (coefficient > new UInt128(0, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF)) {
                throw new OverflowException();
            }

            var lo = (ulong) (coefficient & 0xFFFFFFFFFFFFFFFF);
            var hi = (ulong) (coefficient >> 64);

            var ll = (int) (lo & 0xFFFFFFFF);
            var lh = (int) (lo >> 32);
            var hl = (int) (hi & 0xFFFFFFFF);

            return new decimal(ll, lh, hl, IsNegative(value), (byte) scale);
        }

        #endregion

        #region Writing

        internal void AppendTo(StringBuilder sb, JsonNumberFormat format)
        {
            if (sb is null) {
                throw new ArgumentNullException(nameof(sb));
            }

            // ---------------
            //     Compute
            // ---------------
            bool negative = IsNegative(this);
            var coefficient = GetCoefficient(this);
            int exponent = GetExponent(this);

            bool printZeroExponent;
            int extraZeroCount;
            int scale;

            if (format.ExponentialNotation) {
                printZeroExponent = true;

                if (format.PointPosition <= 0) {
                    scale = MaxSignificandDigits;
                    extraZeroCount = format.PointPosition;
                }
                else if (format.PointPosition < MaxSignificandDigits) {
                    scale = MaxSignificandDigits - format.PointPosition;
                    extraZeroCount = 0;
                }
                else {
                    scale = 0;
                    extraZeroCount = format.PointPosition - MaxSignificandDigits;
                }
            }
            else {
                printZeroExponent = false;

                if (coefficient == UInt128.Zero) {
                    exponent = 0;
                }

                if (exponent <= 0) {
                    scale = -exponent;
                }
                else if (exponent < MaxSignificandDigits) {
                    scale = 0;
                }
                else {
                    scale = MaxSignificandScale;
                }

                extraZeroCount = exponent + scale;
            }

            exponent = exponent + scale - extraZeroCount;

            // -------------------------------------------
            //     Print Integral and Fractional Parts
            // -------------------------------------------

            var significand = coefficient;

            // Skip Fractional Zeros
            scale -= UInt128.Denormalize(ref significand, Math.Min(scale, MaxSignificandScale));

            int numberStart = sb.Length;

            var fractionalDigits = 0;

            // Print Fractional Part in Reverse Order
            if (scale > 0 || format.FractionalDigits != 0 || extraZeroCount < 0) {
                int start = sb.Length;

                for (; scale > 0; scale--) {
                    significand = UInt128.DivRem(significand, 10, out var digit);
                    sb.Append((char) (digit + '0'));
                }

                for (; extraZeroCount < 0; extraZeroCount++) {
                    sb.Append('0');
                }

                fractionalDigits = sb.Length - start;

                sb.Append('.');
            }

            // Print Integral Part in Reverse Order
            {
                int start = sb.Length;

                while (extraZeroCount > 0) {
                    sb.Append('0');
                    extraZeroCount--;
                }

                while (significand != UInt128.Zero) {
                    significand = UInt128.DivRem(significand, 10, out var digit);
                    sb.Append((char) (digit + '0'));
                }

                int digits = sb.Length - start;

                for (; digits < format.IntegralDigits; digits++) {
                    sb.Append('0');
                }

                if (negative && (coefficient != UInt128.Zero || format.PrintMinusZero)) {
                    sb.Append('-');
                }
            }

            StringBuilderUtil.Reverse(sb, numberStart, sb.Length - numberStart);

            // Fill remaining fractional digits with zeros
            for (; fractionalDigits < format.FractionalDigits; fractionalDigits++) {
                sb.Append('0');
            }

            // Print Exponent
            if (exponent != 0 || printZeroExponent) {
                int start = sb.Length;

                bool negativeExponent = exponent < 0;

                if (negativeExponent) {
                    exponent = -exponent;
                }

                while (exponent != 0) {
                    exponent = Math.DivRem(exponent, 10, out int digit);
                    sb.Append((char) (digit + '0'));
                }

                int digits = sb.Length - start;

                for (; digits < format.ExponentDigits; digits++) {
                    sb.Append('0');
                }

                if (negativeExponent) {
                    sb.Append('-');
                }
                else if (format.PrintPlusExponentSign) {
                    sb.Append('+');
                }

                sb.Append(format.UpperCaseExponentSign ? 'E' : 'e');

                StringBuilderUtil.Reverse(sb, start, sb.Length - start);
            }
        }

        public string ToString(JsonNumberFormat format)
        {
            var sb = StringBuilderCache.Acquire();
            AppendTo(sb, format);
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is not null) {
                return ToString(JsonNumberFormat.Parse(format));
            }

            int scale = GetScale(this);

            if (scale < 0) {
                return ToString(ExponentialFormat);
            }

            var coefficient = GetCoefficient(this);

            scale -= UInt128.Denormalize(ref coefficient, MaxSignificandScale);

            if (scale > 34) {
                return ToString(ExponentialFormat);
            }

            return ToString(DecimalFormat);

        }

        #endregion

        #region Parsing

        
        private static bool TryPeek(string s, int index, out char ch)
        {
            if (index >= s.Length) {
                ch = default;
                return false;
            }

            ch = s[index];
            return true;
        }
        
        private static bool TryPeekDigit(string s, int index, out uint digit)
        {
            if (index >= s.Length) {
                digit = default;
                return false;
            }

            char ch = s[index];

            if (ch < '0' || ch > '9') {
                digit = default;
                return false;
            }

            digit = (uint) (ch - '0');
            return true;
        }

        public static JsonDecimal Parse(string s)
        {
            return Parse(s, out _);
        }

        public static JsonDecimal Parse(string s, out JsonNumberFormat fmt)
        {
            if (s is null) {
                throw new ArgumentNullException(nameof(s));
            }

            ParseNumber(s, out var num, out fmt, true);
            return num;
        }

        public static bool TryParse(string s, out JsonDecimal num, out JsonNumberFormat fmt)
        {
            if (s is null) {
                throw new ArgumentNullException(nameof(s));
            }

            return ParseNumber(s, out num, out fmt, false);
        }

        private static bool ParseNumber(string s, out JsonDecimal num, out JsonNumberFormat fmt, bool throwOnError)
        {
            num = default;
            fmt = default;

            var negative = false;
            var coefficient = UInt128.Zero;
            var exponent = 0;

            var significandIntegralDigits = 0;
            var leadingIntegralZeros = 0;
            var trailingIntegralZeros = 0;
            var leadingFractionalZeros = 0;

            var idx = 0;

            // ---------------------
            //     Integral Part    
            // ---------------------
            if (TryPeek(s, idx, out char ch) && ch == '-') {
                idx++;
                negative = true;
            }

            // Leading Integral Zeroes
            while (TryPeekDigit(s, idx, out uint digit) && digit == 0) {
                idx++;
                leadingIntegralZeros++;
            }

            // Integral Significand
            while (TryPeekDigit(s, idx, out uint digit)) {
                idx++;

                if (digit == 0) {
                    trailingIntegralZeros++;
                    continue;
                }

                if (trailingIntegralZeros != 0) {
                    if (trailingIntegralZeros >= MaxSignificandDigits - significandIntegralDigits) {
                        goto overflowError;
                    }

                    coefficient *= UInt128.Pow10(trailingIntegralZeros);
                    significandIntegralDigits += trailingIntegralZeros;
                    trailingIntegralZeros = 0;
                }

                if (significandIntegralDigits >= MaxSignificandDigits) {
                    goto overflowError;
                }

                coefficient = coefficient * 10 + digit;
                significandIntegralDigits++;
            }

            int significandDigits = significandIntegralDigits;

            // Integral Format
            if (leadingIntegralZeros != 0) {
                int integralDigits = leadingIntegralZeros + significandIntegralDigits;

                if (integralDigits < JsonNumberFormat.MinIntegralDigits ||
                    integralDigits > JsonNumberFormat.MaxIntegralDigits) {
                    goto formatError;
                }

                fmt = fmt.SetIntegralDigits(integralDigits);
            }

            // -----------------------
            //     Fractional Part
            // -----------------------
            if (TryPeek(s, idx, out ch) && ch == '.') {
                idx++;

                var trailingFractionalZeros = 0;

                while (TryPeekDigit(s, idx, out uint digit)) {
                    idx++;

                    if (digit == 0) {
                        trailingFractionalZeros++;
                        continue;
                    }

                    if (leadingFractionalZeros == 0) {
                        leadingFractionalZeros = trailingFractionalZeros;
                    }

                    if (trailingIntegralZeros != 0) {
                        if (trailingIntegralZeros >= MaxSignificandDigits - significandDigits) {
                            goto overflowError;
                        }

                        coefficient *= UInt128.Pow10(trailingIntegralZeros);
                        significandDigits += trailingIntegralZeros;
                        significandIntegralDigits += trailingIntegralZeros;
                        trailingIntegralZeros = 0;
                    }

                    if (trailingFractionalZeros != 0) {
                        if (coefficient != UInt128.Zero) {
                            if (trailingFractionalZeros >= MaxSignificandDigits - significandDigits) {
                                goto overflowError;
                            }

                            coefficient *= UInt128.Pow10(trailingFractionalZeros);
                            significandDigits += trailingFractionalZeros;
                        }

                        exponent -= trailingFractionalZeros;
                        trailingFractionalZeros = 0;
                    }

                    if (significandDigits >= MaxSignificandDigits) {
                        goto overflowError;
                    }

                    coefficient = coefficient * 10 + digit;
                    significandDigits++;
                    exponent--;
                }

                // Fractional Format
                if (trailingFractionalZeros != 0) {
                    int fractionalDigits = significandDigits - significandIntegralDigits + trailingFractionalZeros;

                    if (fractionalDigits < JsonNumberFormat.MinFractionalDigits ||
                        fractionalDigits > JsonNumberFormat.MaxFractionalDigits) {
                        goto formatError;
                    }

                    fmt = fmt.WithFractionalDigits(fractionalDigits);
                }
            }

            if (trailingIntegralZeros != 0) {
                exponent += trailingIntegralZeros;
            }

            if (negative && significandDigits == 0) {
                fmt = fmt.WithPrintMinusZero(true);
            }

            // ------------------------ 
            //     Exponential Part     
            // ------------------------ 
            if (TryPeek(s, idx, out ch) && (ch == 'e' || ch == 'E')) {
                idx++;

                var leadingExponentZeros = 0;

                if (ch == 'E') {
                    fmt = fmt.WithUpperCaseExponentSign(true);
                }

                var negativeExponent = false;
                var explicitExponent = 0;

                if (TryPeek(s, idx, out ch) && (ch == '-' || ch == '+')) {
                    idx++;

                    negativeExponent = ch == '-';

                    if (ch == '+') {
                        fmt = fmt.WithPrintPlusExponentSign(true);
                    }
                }

                // Leading Exponent Zeros
                while (TryPeekDigit(s, idx, out uint digit) && digit == 0) {
                    idx++;
                    leadingExponentZeros++;
                }

                var significandExponentDigits = 0;

                while (TryPeekDigit(s, idx, out uint digit)) {
                    idx++;
                    significandExponentDigits++;

                    if (significandExponentDigits > 4) {
                        goto overflowError;
                    }

                    explicitExponent = explicitExponent * 10 + (int) digit;
                }

                if (negativeExponent) {
                    explicitExponent = -explicitExponent;
                }

                exponent += explicitExponent;

                // Exponent Format
                fmt = fmt.WithExponentialNotation(true);

                if (leadingExponentZeros != 0) {
                    int exponentDigits = leadingExponentZeros + significandExponentDigits;

                    if (exponentDigits < JsonNumberFormat.MinExponentDigits ||
                        exponentDigits > JsonNumberFormat.MaxExponentDigits) {
                        goto formatError;
                    }

                    fmt = fmt.WithExponentDigits(exponentDigits);
                }

                if (coefficient != UInt128.Zero) {
                    int pointPosition = significandIntegralDigits != 0 || trailingIntegralZeros != 0
                        ? significandIntegralDigits + trailingIntegralZeros
                        : -leadingFractionalZeros;

                    if (pointPosition < JsonNumberFormat.MinPointPosition ||
                        pointPosition > JsonNumberFormat.MaxPointPosition) {
                        goto formatError;
                    }

                    fmt = fmt.WithPointPosition(pointPosition);
                }
            }

            if (s.Length == 0 || idx < s.Length) {
                goto formatError;
            }

            return PackNumber(negative, coefficient, exponent, out num, throwOnError);

            overflowError:

            if (throwOnError) {
                throw new OverflowException();
            }

            return false;

            formatError:

            if (throwOnError) {
                throw new FormatException();
            }

            return false;
        }

        #endregion

        #region Public Utils

        internal static bool IsInteger(JsonDecimal value)
        {
            var coefficient = GetCoefficient(value);

            if (coefficient == UInt128.Zero) {
                return true;
            }

            int scale = GetScale(value);

            if (scale > MaxSignificandScale) {
                return false;
            }

            if (scale <= 0) {
                return true;
            }

            return coefficient % UInt128.Pow10(scale) == UInt128.Zero;
        }

        internal static bool HasPrecision(JsonDecimal value, int precision)
        {
            if (precision < 0) {
                throw new ArgumentOutOfRangeException(nameof(precision));
            }

            var coefficient = GetCoefficient(value);

            if (coefficient == UInt128.Zero) {
                return true;
            }

            int scale = GetScale(value);

            scale -= UInt128.Denormalize(ref coefficient, MaxSignificandScale);

            return scale <= precision;
        }

        #endregion
    }
}
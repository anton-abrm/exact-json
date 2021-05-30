using System;

using ExactJson.Infra;

namespace ExactJson
{
    public readonly struct JsonNumberFormat : IEquatable<JsonNumberFormat>
    {
        private const int MaxFormatSize = 16;

        private const uint ExponentialNotationFlag = 1U << 31;
        private const uint UpperCaseExponentSignFlag = 1U << 30;
        private const uint PrintPlusExponentSignFlag = 1U << 29;
        private const uint PrintMinusZeroFlag = 1U << 28;

        private const uint ExponentDigitsMask = 0xF << 24;
        private const uint PointPositionMask = 0xFF << 16;
        private const uint IntegralDigitsMask = 0xFF << 8;
        private const uint FractionalDigitsMask = 0xFF;

        internal const int MinIntegralDigits = 1;
        internal const int MaxIntegralDigits = 255;

        internal const int MinFractionalDigits = 0;
        internal const int MaxFractionalDigits = 255;

        internal const int MinExponentDigits = 1;
        internal const int MaxExponentDigits = 15;

        internal const int MinPointPosition = -127;
        internal const int MaxPointPosition = +127;

        public static readonly JsonNumberFormat Decimal =
            new JsonNumberFormat();

        public static readonly JsonNumberFormat Exponential =
            new JsonNumberFormat()
               .WithExponentialNotation(true);

        private static readonly JsonNumberFormat DecimalFloatingPoint =
            new JsonNumberFormat()
               .WithFractionalDigits(1);

        private static readonly JsonNumberFormat ExponentialFloatingPoint =
            new JsonNumberFormat()
               .WithExponentialNotation(true)
               .WithFractionalDigits(1)
               .WithUpperCaseExponentSign(true)
               .WithPrintPlusExponentSign(true);

        private readonly uint _bits;

        private JsonNumberFormat(uint bits)
        {
            _bits = bits;
        }

        public int FractionalDigits => (int) (_bits & 0xFF);
        public int IntegralDigits => (int) ((_bits >> 8) & 0xFF) + 1;
        public int ExponentDigits => (int) ((_bits >> 24) & 0x0F) + 1;
        public int PointPosition => (sbyte) ((_bits >> 16) & 0xFF) + 1;

        public bool UpperCaseExponentSign => (_bits & UpperCaseExponentSignFlag) != 0;
        public bool PrintPlusExponentSign => (_bits & PrintPlusExponentSignFlag) != 0;
        public bool ExponentialNotation => (_bits & ExponentialNotationFlag) != 0;
        public bool PrintMinusZero => (_bits & PrintMinusZeroFlag) != 0;

        #region Equality

        
        private uint GetEquatableData()
        {
            return ExponentialNotation
                ? _bits
                : _bits & 0x1000FFFF;
        }

        public bool Equals(JsonNumberFormat other)
        {
            return GetEquatableData() == other.GetEquatableData();
        }

        public override bool Equals(object obj)
        {
            return obj is JsonNumberFormat other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = 23 * hash + GetEquatableData().GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(JsonNumberFormat left, JsonNumberFormat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(JsonNumberFormat left, JsonNumberFormat right)
        {
            return !(left == right);
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
        
        private static bool TryPeekDigit(string s, int index, out int digit)
        {
            if (index < s.Length) {
                char ch = s[index];

                if (ch >= '0' && ch <= '9') {
                    digit = ch - '0';
                    return true;
                }
            }

            digit = default;
            return false;
        }

        public static JsonNumberFormat Parse(string format)
        {
            if (format is null) {
                throw new ArgumentNullException(nameof(format));
            }

            var bits = 0U;
            var idx = 0;

            // ---------------------
            //     Minus Zero
            // ---------------------
            if (TryPeek(format, idx, out char ch) && ch == '-') {
                idx++;

                bits = SetPrintMinusZeroInternal(bits, true);
            }

            // ---------------------
            //     Integral Part
            // ---------------------
            if (TryPeekDigit(format, idx, out int digit)) {
                idx++;

                int integralDigits = digit;

                while (TryPeekDigit(format, idx, out digit)) {
                    idx++;
                    integralDigits = integralDigits * 10 + digit;

                    if (integralDigits > MaxIntegralDigits) {
                        throw new FormatException();
                    }
                }

                if (integralDigits < MinIntegralDigits) {
                    throw new FormatException();
                }

                bits = SetIntegralDigitsInternal(bits, integralDigits);
            }

            // -----------------------
            //     Fractional Part
            // -----------------------
            if (TryPeek(format, idx, out ch) && ch == '.') {
                idx++;

                if (TryPeekDigit(format, idx, out digit)) {
                    idx++;

                    int fractionalDigits = digit;

                    while (TryPeekDigit(format, idx, out digit)) {
                        idx++;
                        fractionalDigits = fractionalDigits * 10 + digit;

                        if (fractionalDigits > MaxFractionalDigits) {
                            throw new FormatException();
                        }
                    }

                    bits = SetFractionalDigitsInternal(bits, fractionalDigits);
                }
            }

            // ---------------------
            //     Exponent Part
            // ---------------------
            if (TryPeek(format, idx, out ch) && (ch == 'e' || ch == 'E')) {
                idx++;

                bits = SetExponentialNotationInternal(bits, true);
                bits = SetUpperCaseExponentSignInternal(bits, ch == 'E');

                if (TryPeek(format, idx, out ch) && ch == '+') {
                    idx++;
                    bits = SetPrintPlusExponentSignInternal(bits, true);
                }

                if (TryPeekDigit(format, idx, out digit)) {
                    idx++;

                    int exponentDigits = digit;

                    while (TryPeekDigit(format, idx, out digit)) {
                        idx++;
                        exponentDigits = exponentDigits * 10 + digit;

                        if (exponentDigits > MaxExponentDigits) {
                            throw new FormatException();
                        }
                    }

                    if (exponentDigits < MinExponentDigits) {
                        throw new FormatException();
                    }

                    bits = SetExponentDigitsInternal(bits, exponentDigits);
                }

                // ---------------------------
                //     Point Position Part
                // ---------------------------
                if (TryPeek(format, idx, out ch) && ch == ',') {
                    idx++;

                    var sign = 1;

                    if (TryPeek(format, idx, out ch) && ch == '-') {
                        idx++;
                        sign = -1;
                    }

                    if (TryPeekDigit(format, idx, out digit)) {
                        idx++;

                        int pointPosition = sign * digit;

                        while (TryPeekDigit(format, idx, out digit)) {
                            idx++;
                            pointPosition = pointPosition * 10 + sign * digit;

                            if (pointPosition < MinPointPosition ||
                                pointPosition > MaxPointPosition) {
                                throw new FormatException();
                            }
                        }

                        bits = SetPointPositionInternal(bits, pointPosition);
                    }
                }
            }

            if (idx < format.Length) {
                throw new FormatException();
            }

            return new JsonNumberFormat(bits);
        }

        #endregion

        #region Set Methods

        public JsonNumberFormat SetIntegralDigits(int value)
        {
            if (value < MinIntegralDigits || value > MaxIntegralDigits) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new JsonNumberFormat(SetIntegralDigitsInternal(_bits, value));
        }

        public JsonNumberFormat WithFractionalDigits(int value)
        {
            if (value < MinFractionalDigits || value > MaxFractionalDigits) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new JsonNumberFormat(SetFractionalDigitsInternal(_bits, value));
        }

        public JsonNumberFormat WithExponentDigits(int value)
        {
            if (value < MinExponentDigits || value > MaxExponentDigits) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new JsonNumberFormat(SetExponentDigitsInternal(_bits, value));
        }

        public JsonNumberFormat WithPointPosition(int value)
        {
            if (value < MinPointPosition || value > MaxPointPosition) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new JsonNumberFormat(SetPointPositionInternal(_bits, value));
        }

        public JsonNumberFormat WithExponentialNotation(bool value)
        {
            return new JsonNumberFormat(SetExponentialNotationInternal(_bits, value));
        }

        public JsonNumberFormat WithUpperCaseExponentSign(bool value)
        {
            return new JsonNumberFormat(SetUpperCaseExponentSignInternal(_bits, value));
        }

        public JsonNumberFormat WithPrintPlusExponentSign(bool value)
        {
            return new JsonNumberFormat(SetPrintPlusExponentSignInternal(_bits, value));
        }

        public JsonNumberFormat WithPrintMinusZero(bool value)
        {
            return new JsonNumberFormat(SetPrintMinusZeroInternal(_bits, value));
        }

        
        private static uint SetExponentDigitsInternal(uint data, int value)
        {
            return (data & ~ExponentDigitsMask) | ((uint) (value - 1) << 24);
        }
        
        private static uint SetIntegralDigitsInternal(uint data, int value)
        {
            return (data & ~IntegralDigitsMask) | ((uint) (value - 1) << 8);
        }
        
        private static uint SetFractionalDigitsInternal(uint data, int value)
        {
            return (data & ~FractionalDigitsMask) | (uint) value;
        }
        
        private static uint SetPointPositionInternal(uint data, int value)
        {
            return (data & ~PointPositionMask) | ((uint) ((value - 1) & 0xFF) << 16);
        }
        
        private static uint SetExponentialNotationInternal(uint data, bool value)
        {
            return value
                ? data | ExponentialNotationFlag
                : data & ~ExponentialNotationFlag;
        }
        
        private static uint SetUpperCaseExponentSignInternal(uint data, bool value)
        {
            return value
                ? data | UpperCaseExponentSignFlag
                : data & ~UpperCaseExponentSignFlag;
        }
        
        private static uint SetPrintPlusExponentSignInternal(uint data, bool value)
        {
            return value
                ? data | PrintPlusExponentSignFlag
                : data & ~PrintPlusExponentSignFlag;
        }
        
        private static uint SetPrintMinusZeroInternal(uint data, bool value)
        {
            return value
                ? data | PrintMinusZeroFlag
                : data & ~PrintMinusZeroFlag;
        }

        #endregion

        public override string ToString()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var sb = StringBuilderCache.Acquire(MaxFormatSize);

            if (PrintMinusZero) {
                sb.Append('-');
            }

            if (IntegralDigits != 1) {
                sb.Append(IntegralDigits);
            }

            if (FractionalDigits != 0) {
                sb.Append('.');
                sb.Append(FractionalDigits);
            }

            if (ExponentialNotation) {

                sb.Append(UpperCaseExponentSign ? 'E' : 'e');

                if (PrintPlusExponentSign) {
                    sb.Append('+');
                }

                if (ExponentDigits != 1) {
                    sb.Append(ExponentDigits);
                }

                if (PointPosition != 1) {
                    sb.Append(',');
                    sb.Append(PointPosition);
                }
            }

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        #region Number Formats

        public static JsonNumberFormat For(decimal value)
        {
            return Decimal;
        }

        public static JsonNumberFormat For(float value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value == 0.0f) {
                return DecimalFloatingPoint;
            }

            double exp = Math.Log10(Math.Abs(value));

            return exp <= -5 || exp >= 7
                ? ExponentialFloatingPoint
                : DecimalFloatingPoint;
        }

        public static JsonNumberFormat For(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value == 0.0) {
                return DecimalFloatingPoint;
            }

            double exp = Math.Log10(Math.Abs(value));

            return exp <= -5 || exp >= 15
                ? ExponentialFloatingPoint
                : DecimalFloatingPoint;
        }

        [CLSCompliant(false)]
        public static JsonNumberFormat For(sbyte value)
        {
            return Decimal;
        }

        public static JsonNumberFormat For(byte value)
        {
            return Decimal;
        }

        [CLSCompliant(false)]
        public static JsonNumberFormat For(ushort value)
        {
            return Decimal;
        }

        public static JsonNumberFormat For(short value)
        {
            return Decimal;
        }

        [CLSCompliant(false)]
        public static JsonNumberFormat For(uint value)
        {
            return Decimal;
        }

        public static JsonNumberFormat For(int value)
        {
            return Decimal;
        }

        [CLSCompliant(false)]
        public static JsonNumberFormat For(ulong value)
        {
            return Decimal;
        }

        public static JsonNumberFormat For(long value)
        {
            return Decimal;
        }

        #endregion
    }
}
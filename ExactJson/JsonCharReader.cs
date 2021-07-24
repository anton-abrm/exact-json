// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using ExactJson.Infra;

namespace ExactJson
{
    public abstract partial class JsonCharReader : JsonReader
    {
        private const int MaxCachedBuilderCapacity = 1024;

        private const string True = "true";
        private const string False = "false";
        private const string Null = "null";

        private readonly Stack<JsonNodeType> _stack = new Stack<JsonNodeType>();
        private readonly char[] _charBuffer = new char[4];

        private StringBuilder _cachedBuilder;

        private JsonTokenType _tokenType = JsonTokenType.None;

        private int _lineNumber = 1;
        private int _linePosition = 1;
        private int _prevLineNumber = 1;
        private int _prevLinePosition = 1;

        private bool _valueBool;
        private string _valueString;
        private JsonDecimal _valueNumber;
        private JsonNumberFormat _numberFormat;

        internal JsonCharReader() { }

        public int LineNumber => _lineNumber;
        public int LinePosition => _linePosition;

        public override JsonTokenType TokenType => _tokenType;

        public override JsonNumberFormat NumberFormat =>
            _tokenType == JsonTokenType.Number
                ? _numberFormat
                : JsonNumberFormat.Decimal;

        public override bool ValueAsBool
        {
            get
            {
                if (_tokenType == JsonTokenType.Bool) {
                    return _valueBool;
                }

                throw new InvalidOperationException(
                    $"Token type must be {JsonTokenType.Bool}.");
            }
        }

        public override string ValueAsString
        {
            get
            {
                if (_tokenType is JsonTokenType.String or JsonTokenType.Property) {
                    return _valueString ??= _cachedBuilder.ToString();
                }

                throw new InvalidOperationException(
                    $"Token type must be {JsonTokenType.String} or {JsonTokenType.Property}.");
            }
        }

        public override JsonDecimal ValueAsNumber
        {
            get
            {
                if (_tokenType == JsonTokenType.Number) {
                    return _valueNumber;
                }

                throw new InvalidOperationException(
                    $"Token type must be {JsonTokenType.Number} and value should be decimal.");
            }
        }

        private JsonCharReaderException UnexpectedSymbol(char ch)
        {
            string escaped = char.IsControl(ch)
                ? $"U+{(ushort) ch:X4}"
                : ch.ToString();

            return new JsonCharReaderException(_prevLineNumber, _prevLinePosition, $"Unexpected symbol '{escaped}'.");
        }

        protected abstract int ReadCore();
        protected abstract int PeekCore();

        protected abstract long Position { get; set; }
        protected abstract bool CanSeek { get; }

        #region Basic Read Helpers

        private char ReadChar()
        {
            int read = ReadCore();
            if (read < 0) {
                throw new EndOfStreamException();
            }

            var readChar = (char) read;

            _prevLinePosition = _linePosition;
            _prevLineNumber = _lineNumber;

            if (!char.IsControl(readChar)) {
                _linePosition++;
            }
            else if (readChar == '\n') {
                _lineNumber++;
                _linePosition = 1;
            }

            return readChar;
        }

        private bool TryPeekChar(out char result)
        {
            int peeked = PeekCore();

            if (peeked < 0) {
                result = default;
                return false;
            }

            result = (char) peeked;
            return true;
        }

        private void SkipWhiteSpace()
        {
            while (TryPeekChar(out char ch) && char.IsWhiteSpace(ch)) {
                ReadChar();
            }
        }

        #endregion

        #region String Read Helpers

        private char ReadEscapedChar()
        {
            char ch = ReadChar();

            switch (ch) {
                case '\"':
                    return '\"';

                case '\\':
                    return '\\';

                case '/':
                    return '/';

                case 'b':
                    return '\b';

                case 'f':
                    return '\f';

                case 'n':
                    return '\n';

                case 'r':
                    return '\r';

                case 't':
                    return '\t';
            }

            if (ch != 'u') {
                throw UnexpectedSymbol(ch);
            }

            for (var i = 0; i < _charBuffer.Length; i++) {
                _charBuffer[i] = ReadChar();

                if (!HexUtil.IsHex(_charBuffer[i])) {
                    throw UnexpectedSymbol(_charBuffer[i]);
                }
            }

            return (char) ushort.Parse(new string(_charBuffer), NumberStyles.HexNumber);
        }

        private StringBuilder GetBuilder()
        {
            var sb = _cachedBuilder;

            if (sb is null) {
                sb = new StringBuilder();
            }
            else {
                _cachedBuilder = null;
                sb.Clear();
            }

            return sb;
        }

        private void PutBuilderAndSetValueString(StringBuilder sb)
        {
            if (sb.Capacity <= MaxCachedBuilderCapacity) {
                _cachedBuilder = sb;
                _valueString = null;
            }
            else {
                _valueString = sb.ToString();
            }
        }

        private void ReadEscapedString(char ch)
        {
            var sb = GetBuilder();

            char quote = ch;

            while (true) {
                ch = ReadChar();

                if (ch == quote) {
                    break;
                }

                if (char.IsControl(ch)) {
                    throw UnexpectedSymbol(ch);
                }

                if (ch == '\\') {
                    ch = ReadEscapedChar();
                }

                sb.Append(ch);
            }

            PutBuilderAndSetValueString(sb);
        }

        #endregion

        #region Number Read Helpers

        
        private static bool IsNumberChar(char ch)
        {
            if (char.IsDigit(ch)) {
                return true;
            }

            switch (ch) {
                case '-':
                case '+':
                case 'e':
                case 'E':
                case '.':
                    return true;
            }

            return false;
        }

        private void ReadNumber(char ch)
        {
            var sb = GetBuilder();

            int lineNum = _lineNumber;
            int linePos = _linePosition;

            sb.Append(ch);

            while (TryPeekChar(out ch) && IsNumberChar(ch)) {
                ReadChar();
                sb.Append(ch);
            }

            if (!JsonDecimal.TryParse(sb.ToString(), out _valueNumber, out _numberFormat)) {
                throw new JsonCharReaderException(lineNum, linePos, "Invalid number.");
            }

            _tokenType = JsonTokenType.Number;

            PutBuilderAndSetValueString(sb);
        }

        #endregion

        private void ReadAnyImpl(char ch)
        {
            switch (ch) {
                case '{':
                    ReadStartObjectImpl();
                    return;

                case '[':
                    ReadStartArrayImpl();
                    return;

                case '\"':
                    ReadStringImpl(ch);
                    return;

                case 't':
                    ReadTrueImpl();
                    return;

                case 'f':
                    ReadFalseImpl();
                    return;

                case 'n':
                    ReadNullImpl();
                    return;
            }

            if (ch == '-' || char.IsDigit(ch)) {
                ReadNumber(ch);
                return;
            }

            throw UnexpectedSymbol(ch);
        }

        private void ReadNullImpl()
        {
            for (var i = 1; i < Null.Length; i++) {
                char ch = ReadChar();

                if (ch != Null[i]) {
                    throw UnexpectedSymbol(ch);
                }
            }

            _tokenType = JsonTokenType.Null;
        }

        private void ReadFalseImpl()
        {
            for (var i = 1; i < False.Length; i++) {
                char ch = ReadChar();

                if (ch != False[i]) {
                    throw UnexpectedSymbol(ch);
                }
            }

            _valueBool = false;
            _tokenType = JsonTokenType.Bool;
        }

        private void ReadTrueImpl()
        {
            for (var i = 1; i < True.Length; i++) {
                char ch = ReadChar();

                if (ch != True[i]) {
                    throw UnexpectedSymbol(ch);
                }
            }

            _valueBool = true;
            _tokenType = JsonTokenType.Bool;
        }

        private void ReadStringImpl(char ch)
        {
            ReadEscapedString(ch);

            _tokenType = JsonTokenType.String;
        }

        private void ReadStartArrayImpl()
        {
            _stack.Push(JsonNodeType.Array);
            _tokenType = JsonTokenType.StartArray;
        }

        private void ReadEndArrayImpl()
        {
            _stack.Pop();
            _tokenType = JsonTokenType.EndArray;
        }

        private void ReadEndObjectImpl()
        {
            _stack.Pop();
            _tokenType = JsonTokenType.EndObject;
        }

        private void ReadStartObjectImpl()
        {
            _stack.Push(JsonNodeType.Object);
            _tokenType = JsonTokenType.StartObject;
        }

        private void ReadPropertyImpl(char ch)
        {
            ReadEscapedString(ch);

            SkipWhiteSpace();

            ch = ReadChar();

            if (ch != ':') {
                throw UnexpectedSymbol(ch);
            }

            _tokenType = JsonTokenType.Property;
        }

        private void ReadAnyOrEndArrayImpl(char ch)
        {
            bool hasComma = ch == ',';

            if (hasComma) {
                SkipWhiteSpace();
                ch = ReadChar();
            }

            if (ch == ']') {
                ReadEndArrayImpl();
                return;
            }

            if (_tokenType != JsonTokenType.StartArray && !hasComma) {
                throw UnexpectedSymbol(ch);
            }

            ReadAnyImpl(ch);
        }

        private void ReadPropertyOrEndObjectImpl(char ch)
        {
            bool hasComma = ch == ',';

            if (hasComma) {
                SkipWhiteSpace();
                ch = ReadChar();
            }

            if (ch == '}') {
                ReadEndObjectImpl();
                return;
            }

            if (ch != '\'' && ch != '\"') {
                throw UnexpectedSymbol(ch);
            }

            if (_tokenType != JsonTokenType.StartObject && !hasComma) {
                throw UnexpectedSymbol(ch);
            }

            ReadPropertyImpl(ch);
        }

        public override bool Read()
        {
            SkipWhiteSpace();

            if (_stack.Count == 0) {
                if (TryPeekChar(out _)) {
                    ReadAnyImpl(ReadChar());
                    return true;
                }

                _tokenType = JsonTokenType.None;
                return false;
            }

            char ch = ReadChar();

            switch (_tokenType) {
                case JsonTokenType.Property:
                    ReadAnyImpl(ch);
                    return true;

                case JsonTokenType.StartObject:
                    ReadPropertyOrEndObjectImpl(ch);
                    return true;

                case JsonTokenType.StartArray:
                    ReadAnyOrEndArrayImpl(ch);
                    return true;
            }

            if (_stack.Peek() == JsonNodeType.Object) {
                ReadPropertyOrEndObjectImpl(ch);
            }
            else {
                ReadAnyOrEndArrayImpl(ch);
            }

            return true;
        }

        public override bool CanSaveState
            => CanSeek;

        public override JsonReaderState SaveState()
        {
            if (!CanSaveState) {
                throw new NotSupportedException();
            }

            return new State(this);
        }
    }
}
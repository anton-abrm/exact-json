// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExactJson
{
    public sealed class JsonTextWriter : JsonWriter
    {
        private enum JsonContainerType
        {
            Object,
            Array,
        }

        private const int MaxCachedBuilderCapacity = 1024;

        private const string HexUpper = "0123456789ABCDEF";
        private const string HexLower = "0123456789abcdef";

        private readonly Stack<JsonContainerType> _stack = new Stack<JsonContainerType>();
        private readonly TextWriter _writer;
        private readonly bool _closeOutput;

        private StringBuilder _builder;
        private JsonTokenType _tokenType = JsonTokenType.None;
        private int _indentSize = 2;
        private string _newLine = "\n";
        private string _jsonSeparator = "\n";

        private bool _disposed;

        public JsonTextWriter(TextWriter writer) 
            : this(writer, false) { }

        public JsonTextWriter(TextWriter writer, bool closeOutput)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            _writer = writer;
            _closeOutput = closeOutput;
        }

        public bool Formatted { get; set; }

        public int IndentSize
        {
            get => _indentSize;
            set
            {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _indentSize = value;
            }
        }

        public string NewLine
        {
            get => _newLine;
            set
            {
                if (value is null) {
                    throw new ArgumentNullException(nameof(value));
                }
                
                if (value is not ("\n" or "\r\n")) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _newLine = value;
            }
        }
        
        public string JsonSeparator
        {
            get => _jsonSeparator;
            set
            {
                if (value is null) {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value == string.Empty) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                if (value.Any(ch => !char.IsWhiteSpace(ch))) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                _jsonSeparator = value;
            }
        }

        public bool EscapeSolidus { get; set; }

        public bool EscapeNonAsciiChars { get; set; }

        public bool WriteHexInLowerCase { get; set; }

        private StringBuilder GetBuilder()
        {
            var sb = _builder;

            if (sb is null) {
                sb = new StringBuilder();
            }
            else {
                _builder = null;
                sb.Clear();
            }

            return sb;
        }

        private void PutBuilder(StringBuilder sb)
        {
            if (sb.Capacity <= MaxCachedBuilderCapacity) {
                _builder = sb;
            }
        }

        
        private void EnsureNotClosed()
        {
            if (_disposed) {
                throw new ObjectDisposedException("Object already disposed.");
            }
        }

        public override void WriteProperty(string name)
        {
            EnsureNotClosed();

            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            CheckStateForToken(JsonTokenType.Property);

            WriteCommaIfNeeded();

            WriteNewLineIfFormatted();

            WriteEscapedString(name);

            _writer.Write(':');

            _tokenType = JsonTokenType.Property;
        }

        public override void WriteNull()
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.Null);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            _writer.Write("null");

            _tokenType = JsonTokenType.Null;
        }

        public void WriteRaw(string value)
        {
            EnsureNotClosed();
            
            _writer.Write(value);
        }

        public override void WriteBool(bool value)
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.Bool);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            _writer.Write(value ? "true" : "false");

            _tokenType = JsonTokenType.Bool;
        }

        public override void WriteNumber(JsonDecimal value, JsonNumberFormat format)
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.Number);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            var sb = GetBuilder();

            value.AppendTo(sb, format);

            for (var i = 0; i < sb.Length; i++) {
                _writer.Write(sb[i]);
            }

            PutBuilder(sb);

            _tokenType = JsonTokenType.Number;
        }

        public override void WriteString(string value)
        {
            EnsureNotClosed();

            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            CheckStateForToken(JsonTokenType.String);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            WriteEscapedString(value);

            _tokenType = JsonTokenType.String;
        }

        public override void WriteStartObject()
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.StartObject);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            _writer.Write('{');

            _stack.Push(JsonContainerType.Object);
            _tokenType = JsonTokenType.StartObject;
        }

        public override void WriteEndObject()
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.EndObject);

            _stack.Pop();

            if (_tokenType != JsonTokenType.StartObject) {
                WriteNewLineIfFormatted();
            }

            _tokenType = JsonTokenType.EndObject;

            _writer.Write('}');
        }

        public override void WriteStartArray()
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.StartArray);

            WriteCommaIfNeeded();
            WriteNewLineOrWhitespaceIfNeeded();

            _writer.Write('[');

            _stack.Push(JsonContainerType.Array);
            _tokenType = JsonTokenType.StartArray;
        }

        public override void WriteEndArray()
        {
            EnsureNotClosed();
            CheckStateForToken(JsonTokenType.EndArray);

            _stack.Pop();

            if (_tokenType != JsonTokenType.StartArray) {
                WriteNewLineIfFormatted();
            }

            _tokenType = JsonTokenType.EndArray;

            _writer.Write(']');
        }

        private void WriteWhitespaceIfFormatted()
        {
            if (!Formatted) {
                return;
            }

            _writer.Write(' ');
        }

        private void WriteNewLineIfFormatted()
        {
            if (!Formatted) {
                return;
            }

            _writer.Write(_newLine);

            int charCount = _indentSize * _stack.Count;

            for (var i = 0; i < charCount; i++) {
                _writer.Write(' ');
            }
        }

        private void WriteNewLineOrWhitespaceIfNeeded()
        {
            if (_stack.Count == 0) {
                if (_tokenType != JsonTokenType.None) {
                    _writer.Write(_jsonSeparator);
                }
            }
            else {

                switch (_stack.Peek()) {

                    case JsonContainerType.Array:
                        WriteNewLineIfFormatted();
                        break;

                    case JsonContainerType.Object:
                        WriteWhitespaceIfFormatted();
                        break;
                }
            }
        }

        private void WriteCommaIfNeeded()
        {
            if (_stack.Count == 0) {
                return;
            }

            switch (_tokenType) {

                case JsonTokenType.Null:
                case JsonTokenType.Bool:
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    _writer.Write(',');
                    break;
            }
        }

        private void WriteEscapedString(string s)
        {
            _writer.Write('\"');

            foreach (char ch in s) {

                switch (ch) {

                    case '\"':
                        _writer.Write("\\\"");
                        continue;

                    case '\\':
                        _writer.Write("\\\\");
                        continue;

                    case '\b':
                        _writer.Write("\\b");
                        continue;

                    case '\f':
                        _writer.Write("\\f");
                        continue;

                    case '\n':
                        _writer.Write("\\n");
                        continue;

                    case '\r':
                        _writer.Write("\\r");
                        continue;

                    case '\t':
                        _writer.Write("\\t");
                        continue;

                    case '/':
                        _writer.Write(EscapeSolidus ? "\\/" : "/");
                        continue;
                }

                if (char.IsControl(ch) || ch >= 128 && EscapeNonAsciiChars) {

                    string chars = WriteHexInLowerCase
                        ? HexLower
                        : HexUpper;

                    _writer.Write("\\u");

                    int hi = (ch & 0xFF00) >> 8;
                    int lo = (ch & 0x00FF) >> 0;

                    _writer.Write(chars[(hi & 0xF0) >> 4]);
                    _writer.Write(chars[(hi & 0x0F) >> 0]);

                    _writer.Write(chars[(lo & 0xF0) >> 4]);
                    _writer.Write(chars[(lo & 0x0F) >> 0]);

                    continue;
                }

                _writer.Write(ch);
            }

            _writer.Write('\"');
        }

        private void CheckStateForToken(JsonTokenType tokenType)
        {
            switch (tokenType) {

                case JsonTokenType.Property:
                case JsonTokenType.EndObject:
                {
                    if (_stack.Count != 0 &&
                        _stack.Peek() == JsonContainerType.Object &&
                        _tokenType != JsonTokenType.Property) {
                        return;
                    }

                    break;
                }

                case JsonTokenType.Null:
                case JsonTokenType.Bool:
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                {
                    if (_stack.Count != 0 &&
                        _stack.Peek() == JsonContainerType.Object &&
                        _tokenType != JsonTokenType.Property) {
                        break;
                    }

                    return;
                }

                case JsonTokenType.EndArray:
                {
                    if (_stack.Count > 0 &&
                        _stack.Peek() == JsonContainerType.Array) {
                        return;
                    }

                    break;
                }
            }

            throw new InvalidOperationException(
                $"Token {tokenType} in the current state would result in an invalid JSON document.");
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) {
                return;
            }

            if (disposing) {
                if (_closeOutput) {
                    _writer.Dispose();
                }
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
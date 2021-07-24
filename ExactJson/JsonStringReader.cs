// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson
{
    public sealed class JsonStringReader : JsonCharReader
    {
        private readonly string _value;
        
        private int _position;

        public JsonStringReader(string value)
            => _value = value ?? throw new ArgumentNullException(nameof(value));

        protected override bool CanSeek
            => true;

        protected override int ReadCore()
        {
            if (_position < _value.Length) {
                return _value[_position++];
            }

            return -1;
        }

        protected override int PeekCore()
        {
            if (_position < _value.Length) {
                return _value[_position];
            }

            return -1;
        }

        protected override long Position
        {
            get => _position;
            set => _position = checked ((int) value);
        }
    }
}
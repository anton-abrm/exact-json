// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;

namespace ExactJson
{
    public sealed class JsonStreamReader : JsonCharReader
    {
        private readonly Stream _stream;
        private readonly Decoder _decoder;

        private readonly char[] _chars = new char[1];
        private readonly byte[] _bytes = new byte[1];

        private int _charsRead;
        private int _bytesRead;

        protected override bool CanSeek 
            => _stream.CanSeek;

        protected override long Position
        {
            get => _stream.Position - _bytesRead;
            set
            {
                _stream.Position = value;
                _bytesRead = 0;
                _charsRead = 0;
            }
        }

        public JsonStreamReader(Stream stream) :
            this(stream, null)
        { }

        public JsonStreamReader(Stream stream, Encoding encoding)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _decoder = (encoding ?? Encoding.UTF8).GetDecoder();
        }
        
        public bool CloseInput { get; set; }

        private void EnsureBuffered()
        {
            if (_charsRead != 0) {
                return;
            }

            _bytesRead = 0;

            while (true) {
                
                var bytesRead = _stream.Read(_bytes, 0, 1);

                if (bytesRead != 0) {
                    
                    _bytesRead += bytesRead;

                    int charsRead = _decoder.GetChars(_bytes, 0, 1, _chars, 0);

                    if (charsRead == 0 || _chars[0] == '\uFEFF') {
                        continue;
                    }

                    _charsRead = charsRead;
                }

                break;
            }
        }

        protected override int ReadCore()
        {
            EnsureBuffered();

            if (_charsRead == 0) {
                return -1;
            }

            _charsRead = 0;
            _bytesRead = 0;

            return _chars[0];
        }

        protected override int PeekCore()
        {
            EnsureBuffered();

            if (_charsRead != 0) {
                return _chars[0];
            }

            return -1;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (CloseInput) {
                    _stream.Dispose();
                }
            }
            
            base.Dispose(disposing);
        }
    }
}
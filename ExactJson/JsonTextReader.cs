using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ExactJson
{
    public sealed class JsonTextReader : JsonCharReader
    {
        private readonly TextReader _reader;

        public JsonTextReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }
        
        public bool CloseInput { get; set; }

        protected override int ReadCore()
            => _reader.Read();

        protected override int PeekCore()
            => _reader.Peek();

        protected override bool CanSeek 
            => false;

        [ExcludeFromCodeCoverage]
        protected override long Position
        {
            get => throw new NotSupportedException(); 
            set => throw new NotSupportedException();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (CloseInput) {
                    _reader.Dispose();
                }
            }
            
            base.Dispose(disposing);
        }
    }
}
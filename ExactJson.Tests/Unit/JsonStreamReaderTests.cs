using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonStreamReaderTests
    {
        #region Dispose
        
        [Test]
        public void Dispose_WithCloseInput_ClosesInternalReader()
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("[]"));
        
            var jr = new JsonStreamReader(ms)
            {
                CloseInput = true
            };
        
            jr.Dispose();

            Assert.Throws<ObjectDisposedException>(() => ms.ReadByte());
        }
        
        [Test]
        public void Dispose_WithoutCloseInput_DoesNotCloseInternalReader()
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("[]"));
        
            var jr = new JsonStreamReader(ms)
            {
                CloseInput = false
            };
        
            jr.Dispose();

            Assert.That(ms.ReadByte(), Is.EqualTo((int)'['));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Read_Bom(bool emitBom)
        {
            using var stream = new MemoryStream();
            
            using var sw = new StreamWriter(stream, new UTF8Encoding(emitBom));
            
            sw.Write('[');
            sw.Flush();
            
            stream.Position = 0;

            using var jr = new JsonStreamReader(stream);
            
            Assert.That(jr.MoveToToken(), Is.EqualTo(JsonTokenType.StartArray));
        }

        #endregion
    }
}
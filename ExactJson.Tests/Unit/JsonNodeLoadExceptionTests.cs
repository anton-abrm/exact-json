using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonNodeLoadExceptionTests
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            var originalException = new JsonNodeLoadException(
                JsonPointer.Parse("/1/2/3"),
                new JsonCharReaderException(1, 2));

            var formatter = new BinaryFormatter();

            using var stream = new MemoryStream();

            #pragma warning disable SYSLIB0011
            #pragma warning disable 618
            
            formatter.Serialize(stream, originalException);
            
            stream.Position = 0;
            
            var deserializedException = (JsonNodeLoadException) formatter.Deserialize(stream);
            
            #pragma warning restore 618
            #pragma warning restore SYSLIB0011

            Assert.That(deserializedException, Is.Not.Null);
            Assert.That(deserializedException.Message, Is.EqualTo("Unable to load node. Pointer: '/1/2/3' Position: (1:2)"));
            Assert.That(deserializedException.Pointer, Is.EqualTo("/1/2/3"));
            Assert.That(deserializedException.InnerException, Is.InstanceOf(typeof(JsonCharReaderException)));
        }

        [Test]
        public void Ctor_NullPointer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonNodeLoadException(null, new JsonCharReaderException(1, 2));
            });

        }

        [Test]
        public void Ctor_NullReaderException_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonNodeLoadException(JsonPointer.Root(), null);
            });

        }
    }
}
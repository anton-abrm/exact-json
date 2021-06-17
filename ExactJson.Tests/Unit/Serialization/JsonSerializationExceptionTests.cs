using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class JsonSerializationExceptionTests
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            var originalException = new JsonSerializationException(
                JsonPointer.Parse("/1/2/3"),
                new JsonCharReaderException(1, 2));

            var formatter = new BinaryFormatter();

            using var stream = new MemoryStream();

#pragma warning disable SYSLIB0011
#pragma warning disable 618
            
            formatter.Serialize(stream, originalException);
            
            stream.Position = 0;
            
            var deserializedException = (JsonSerializationException) formatter.Deserialize(stream);
            
#pragma warning restore 618
#pragma warning restore SYSLIB0011

            Assert.That(deserializedException, Is.Not.Null);
            Assert.That(deserializedException.Message, Is.EqualTo("Serialization error occured. Pointer: '/1/2/3' Position: (1:2)"));
            Assert.That(deserializedException.Pointer, Is.EqualTo("/1/2/3"));
            Assert.That(deserializedException.InnerException, Is.InstanceOf(typeof(JsonCharReaderException)));
        }

        [Test]
        public void Ctor_NullPointer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonSerializationException(null, new JsonCharReaderException(1, 2));
            });

        }

        [Test]
        public void Ctor_NullReaderException_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonSerializationException(JsonPointer.Root(), null);
            });

        }
    }
}
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonCharReaderExceptionTests
    {
        [Test]
        public void SerializationCtorAndGetObjectData_ByDefault_WritesAndReadsState()
        {
            var originalException = new JsonCharReaderException(1, 2, "foo");

            var formatter = new BinaryFormatter();

            using var stream = new MemoryStream();

            #pragma warning disable SYSLIB0011
            #pragma warning disable 618
            
            formatter.Serialize(stream, originalException);

            stream.Position = 0;

            var deserializedException = (JsonCharReaderException) formatter.Deserialize(stream);

            #pragma warning restore 618
            #pragma warning restore SYSLIB0011
            
            Assert.That(deserializedException, Is.Not.Null);
            Assert.That(deserializedException.Message, Is.EqualTo("foo Position: (1:2)"));
            Assert.That(deserializedException.LineNumber, Is.EqualTo(1));
            Assert.That(deserializedException.LinePosition, Is.EqualTo(2));
        }
    }
}
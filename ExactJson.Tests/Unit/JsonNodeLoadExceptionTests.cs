using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonNodeLoadExceptionTests
    {
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
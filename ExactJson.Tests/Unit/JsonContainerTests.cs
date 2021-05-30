using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonContainerTests
    {
        [Test]
        public void CreateWriter()
        {
            Assert.That(new JsonArray().CreateWriter(), Is.Not.Null);
        }
    }
}
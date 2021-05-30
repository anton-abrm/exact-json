using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonNodeDiffTests
    {
        [Test]
        public void Constructor()
        {
            var diff = new JsonNodeDiff(
                JsonPointer.Root(),
                JsonBool.Create(false),
                JsonBool.Create(true));

            Assert.That(diff.Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diff.Other, Is.EqualTo(JsonBool.Create(true)));
            Assert.That(diff.Self, Is.EqualTo(JsonBool.Create(false)));
        }
    }
}
using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    internal class JsonPropertyTests
    {
        [Test]
        public void Ctor()
        {
            var value = JsonString.Create("bar");

            var property = new JsonProperty("foo", value);

            Assert.That(property.Name, Is.EqualTo("foo"));
            Assert.That(property.Value, Is.SameAs(value));
        }

        [TestCase("foo", "bar", "foo", "bar", true)]
        [TestCase("foo", "bar", "foo", "baz", false)]
        [TestCase("foo", "bar", "foz", "bar", false)]
        [TestCase("foo", "bar", "foz", "baz", false)]
        public void Equality(string name1, string value1,
                             string name2, string value2,
                             bool expectedEquals)
        {
            var a = new JsonProperty(name1, value1);
            var b = new JsonProperty(name2, value2);

            Assert.That(a.Equals((object) b), Is.EqualTo(expectedEquals));
            Assert.That(a == b, Is.EqualTo(expectedEquals));
            Assert.That(a != b, Is.EqualTo(!expectedEquals));

            if (expectedEquals) {
                Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            }
        }

        [Test]
        public void Empty()
        {
            Assert.That(JsonProperty.Empty.Name, Is.Null);
            Assert.That(JsonProperty.Empty.Value, Is.Null);
        }
    }
}
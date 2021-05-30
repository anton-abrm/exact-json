using Moq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonValueTests
    {
        [Test]
        public void DeepEquals_GivenJsonValue_CallsEqualsWithValue()
        {
            var mock = new Mock<JsonValue>();

            mock.Object.DeepEquals(null);

            mock.Verify(v => v.Equals(null));
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Clone_ByDefault_ReturnsSameInstance()
        {
            var mock = new Mock<JsonValue>();

            var original = mock.Object;
            var clone = original.Clone();

            Assert.That(original, Is.SameAs(clone));
        }
    }
}
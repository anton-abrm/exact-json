using System;

using Moq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonBoolTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Create_GivenInput_CreatesInstanceWithInput(bool value)
        {
            var instance = JsonBool.Create(value);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Value, Is.EqualTo(value));
        }

        [Test]
        public void NodeType_ByDefault_ReturnsBoolNodeType()
        {
            var instance = JsonBool.Create(true);

            Assert.That(instance.NodeType, Is.EqualTo(JsonNodeType.Bool));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Write_CreatedOnInput_CallsWriteBoolWithValue(bool value)
        {
            var mock = new Mock<JsonWriter>();

            JsonBool.Create(value).WriteTo(mock.Object);

            mock.Verify(w => w.WriteBool(value), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Write_GivenNull_ThrowsArgumentNullException()
        {
            var value = JsonBool.Create(true);

            Assert.Throws<ArgumentNullException>(
                () => value.WriteTo((JsonWriter) null));
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void Equals_GivenInputs_ReturnsResult(bool left, bool right, bool expectedResult)
        {
            object b1 = JsonBool.Create(left);
            object b2 = JsonBool.Create(right);

            bool result = b1.Equals(b2);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Equals_GivenInputAndNull_ReturnsFalse(bool value)
        {
            object b1 = JsonBool.Create(value);

            bool result = b1.Equals(null);

            Assert.That(result, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GetHashCode_GivenInputs_ReturnsHashCode(bool value)
        {
            int hashCode = JsonBool.Create(value).GetHashCode();
            int expectedHashCode = unchecked(17 * 23 + value.GetHashCode());

            Assert.That(hashCode, Is.EqualTo(expectedHashCode));
        }
    }
}
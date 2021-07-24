// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Moq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    internal sealed class JsonNullTests
    {
        [Test]
        public void Create()
        {
            var instance = JsonNull.Create();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.NodeType, Is.EqualTo(JsonNodeType.Null));
        }

        [Test]
        public void Equals_Instances()
        {
            object n1 = JsonNull.Create();
            object n2 = JsonNull.Create();

            Assert.That(n1.Equals(n2), Is.True);
        }

        [Test]
        public void Equals_InstanceAndNull()
        {
            object n1 = JsonNull.Create();

            Assert.That(n1.Equals(null), Is.False);
        }

        [Test]
        public void HashCode()
        {
            int hash = JsonNull.Create().GetHashCode();

            Assert.That(hash, Is.EqualTo(17));
        }

        [Test]
        public void Write_CallsWriteNull()
        {
            var mock = new Mock<JsonWriter>();

            JsonNull.Create().WriteTo(mock.Object);

            mock.Verify(w => w.WriteNull(), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Write_Null_ThrowsArgumentNullException()
        {
            var value = JsonNull.Create();

            Assert.Throws<ArgumentNullException>(
                () => value.WriteTo((JsonWriter) null));
        }
    }
}
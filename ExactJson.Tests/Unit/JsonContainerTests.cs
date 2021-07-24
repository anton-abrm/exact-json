// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

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
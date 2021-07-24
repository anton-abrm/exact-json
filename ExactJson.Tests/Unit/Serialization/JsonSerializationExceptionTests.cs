// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class JsonSerializationExceptionTests
    {
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
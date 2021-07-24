// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class StructTests
    {
        private struct Complex
        {
            [JsonNode("r")]
            public double Real { get; set; }

            [JsonNode("i")]
            public double Imaginary { get; set; }
        }

        [TestCase(typeof(Complex), "{\"r\":1.0,\"i\":2.0}")]
        public void Serialize_Struct(Type type, string json)
        {
            var serializer = new JsonSerializer();

            var obj = serializer.Deserialize<Complex>(json);

            var restored = serializer.Serialize<Complex>(obj);

            Assert.That(restored, Is.EqualTo(json));
        }
    }
}
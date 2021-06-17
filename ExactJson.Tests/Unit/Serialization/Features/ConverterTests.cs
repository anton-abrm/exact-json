using System;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class ConverterTests
    {
        private sealed class UriConverter : JsonConverter
        {
            public override void Write(JsonWriter output, object value, JsonConverterContext context)
            {
                output.WriteString(((Uri) value).ToString());
            }

            public override object Read(JsonReader input, JsonConverterContext context)
            {
                return new Uri(input.ReadString());
            }
        }

        private sealed class Profile
        {
            [JsonNode("link")]
            [JsonConverter(typeof(UriConverter))]
            public Uri Link { get; set; }
        }

        [Test]
        public void Serialize_PropertyWithConverter_UsesConverter()
        {
            var p = new Profile
            {
                Link = new Uri("http://localhost"),
            };

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Profile>(p);

            Assert.That(result, Is.EqualTo("{\"link\":\"http://localhost/\"}"));
        }

        [Test]
        public void Deserialize_PropertyWithConverter_UsesConverter()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Profile>("{\"link\":\"http://localhost/\"}");

            Assert.That(result.Link, Is.EqualTo(new Uri("http://localhost")));
        }
    }
}
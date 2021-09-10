// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Functional
{
    public class JsonExamples
    {
        [Test]
        public void Diff()
        {
            var node1 = JsonNode.Parse("[1E-30, 0.29999999999999999]");
            var node2 = JsonNode.Parse("[1E-30, 0.3]");

            var diffs = node1.Diff(node2);

            foreach (var diff in diffs) {
                Console.WriteLine($"{ diff.Pointer } | { diff.Self } | { diff.Other }");
            }
            
            // Output:
            // /1 | 0.29999999999999999 | 0.3

            Assert.That(diffs.Length, Is.EqualTo(1));
            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Parse("/1")));
            Assert.That(diffs[0].Self, Is.EqualTo(JsonNode.Parse("0.29999999999999999")));
            Assert.That(diffs[0].Other, Is.EqualTo(JsonNode.Parse("0.3")));
        }
        
        [Test]
        public void Equality()
        {
            var node1 = JsonNode.Parse("[1E-29, 0.29999999999999999]");
            var node2 = JsonNode.Parse("[1E-30, 0.3]");

            var nodesEqual = node1.Equals(node2);

            Console.WriteLine($"Equal: {nodesEqual}");  
            
            // Output: Equal: False
            
            Assert.That(nodesEqual, Is.False);
        }
        
        [Test]
        public void NumberFormatPreserving()
        {
            var result = JsonNode.Parse("[1.00, 0.000, 1E+2]").ToString();

            Console.WriteLine(result); 
            
            // Output: [1.00,0.000,1E+2]
            
            Assert.That(result, Is.EqualTo("[1.00,0.000,1E+2]"));
        }

        public sealed class Numbers
        {
            [JsonNode, JsonFormat("3")] // 010
            public int A { get; set; } = 10; 
            
            [JsonNode, JsonFormat(".2")] // 10.00
            public int B { get; set; } = 10; 
            
            [JsonNode, JsonFormat("E+2")] // 1E+01
            public int C { get; set; } = 10; 
            
            [JsonNode, JsonFormat("3.2E+2")] // 001.00E+01
            public int D { get; set; } = 10; 
        }
        
        [Test]
        public void CustomNumberFormat()
        {
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<Numbers>(new Numbers());
            
            Console.WriteLine(json);
            
            // Output:
            // {"A":010,"B":10.00,"C":1E+01,"D":001.00E+01}

            Assert.That(json, Is.EqualTo("{\"A\":010,\"B\":10.00,\"C\":1E+01,\"D\":001.00E+01}"));
        }
        
        [Test]
        public void SaveAndRestoreReaderState()
        {
            var reader = new JsonStringReader("[1, 2, 3]");
            
            reader.Read();
            reader.Read();

            Console.WriteLine($"Value: {reader.Value}"); 
            
            // Output: 1
            
            Assert.That((int) reader.ValueAsNumber, Is.EqualTo(1));
            
            var state = reader.SaveState();

            while (reader.Read()) {
            }

            state.Restore();

            Console.WriteLine($"Value: {reader.Value}"); 
            
            // Output: 1
            
            Assert.That((int) reader.ValueAsNumber, Is.EqualTo(1));
        }
        
        [Test]
        public void JsonLines()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);
            
            jw.WriteNumber(1);
            jw.WriteNumber(2);
            jw.WriteNumber(3);

            var result = sw.ToString();
            
            Console.WriteLine(result);
            
            // Output:
            // 1
            // 2
            // 3
            
            Assert.That(result, Is.EqualTo("1\n2\n3"));
        }

        [JsonTuple]
        public struct Point
        {
            [JsonNode]
            public double X { get; set; }
            
            [JsonNode]
            public double Y { get; set; }
        }

        [Test]
        public void SerializeObjectsAsTuple()
        {
            var point = new Point {
                X = 1,
                Y = 2
            };
            
            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Point>(point);

            Console.WriteLine(result); 
            
            // Output: [1.0,2.0]
            
            Assert.That(result, Is.EqualTo("[1.0,2.0]"));
        }
    }
}
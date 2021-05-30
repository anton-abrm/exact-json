using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonObjectTests
    {
        [Test]
        public void Ctor()
        {
            var jo = new JsonObject();

            Assert.That(jo.Count, Is.EqualTo(0));
            Assert.That(jo.NodeType, Is.EqualTo(JsonNodeType.Object));
        }

        [Test]
        public void WriteTo_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new JsonObject().WriteTo((JsonWriter) null);
            });
        }
        
        [Test]
        public void First()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.First, Is.EqualTo(new JsonProperty("1", 1)));
        }
        
        [Test]
        public void Next()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.Next("2"), Is.EqualTo(new JsonProperty("3", 3)));
        }
        
        [Test]
        public void Next_Last()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.Next("3"), Is.Null);
        }
        
        [Test]
        public void Next_PropertyNameIsNull_ThrowsArgumentNullException()
        {
            var jo = new JsonObject();
            
            Assert.Throws<ArgumentNullException>(() => jo.Next(null));
        }
        
        [Test]
        public void Next_NotPresentProperty_ThrowsInvalidOperationException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.Throws<InvalidOperationException>(() => jo.Next("4"));
        }
        
        [Test]
        public void Previous()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.Previous("2"), Is.EqualTo(new JsonProperty("1", 1)));
        }
        
        [Test]
        public void Previous_First()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.Previous("1"), Is.Null);
        }
        
        [Test]
        public void Previous_PropertyNameIsNull_ThrowsArgumentNullException()
        {
            var jo = new JsonObject();
            
            Assert.Throws<ArgumentNullException>(() => jo.Previous(null));
        }
        
        [Test]
        public void Previous_NotPresentProperty_ThrowsInvalidOperationException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.Throws<InvalidOperationException>(() => jo.Previous("4"));
        }
        
        [Test]
        public void First_Empty_ReturnsNull()
        {
            var jo = new JsonObject();
            
            Assert.That(jo.First, Is.Null);
        }
        
        [Test]
        public void Last()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };
            
            Assert.That(jo.Last, Is.EqualTo(new JsonProperty("3", 3)));
        }
        
        [Test]
        public void Last_Empty_ReturnsNull()
        {
            var jo = new JsonObject();
            
            Assert.That(jo.Last, Is.Null);
        }

        [Test]
        public void WriteTo()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
            };

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jo.WriteTo(jw);

            Assert.That(sw.ToString(), Is.EqualTo("{\"1\":1,\"2\":2}"));
        }

        [Test]
        public void Add_NameValue()
        {
            var jo = new JsonObject();

            jo.Add("1", 1);
            jo.Add("2", 2);

            Assert.That(jo.Count, Is.EqualTo(2));

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 1),
                new JsonProperty("2", 2),
            }));
        }
        
        [Test]
        public void Add_NameValue_NameNull_ThrowsArgumentNullException()
        {
            var jo = new JsonObject();
            
            Assert.Throws<ArgumentNullException>(() =>
            {
                jo.Add(null, 1);
            });
        }
        
        [Test]
        public void Add_NameValue_ValueNull_ThrowsArgumentNullException()
        {
            var jo = new JsonObject();
            
            Assert.Throws<ArgumentNullException>(() =>
            {
                jo.Add("null", null);
            });
        }
        
        [Test]
        public void Add_NameValue_AlreadyExists_ThrowsInvalidOperationException()
        {
            var jo = new JsonObject() {
                { "1", 1 }
            };
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                jo.Add("1", 2);
            });
        }

        [Test]
        public void Indexer_Get()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
            };

            Assert.That(jo["1"], Is.EqualTo((JsonNode) 1));
            Assert.That(jo["2"], Is.Null);
        }

        [Test]
        public void Indexer_Get_Null_ThrowsArgumentNullException()
        {
            var jo = new JsonObject();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = jo[null];
            });
        }

        [Test]
        public void Indexer_Set()
        {
            var jo = new JsonObject();

            jo["1"] = 1;
            jo["2"] = 2;
            jo["1"] = 3;

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 3),
                new JsonProperty("2", 2),
            }));
        }

        [Test]
        public void Indexer_Set_ValueNull_ThrowsArgumenNullException()
        {
            var jo = new JsonObject();

            Assert.Throws<ArgumentNullException>(() =>
            {
                jo["1"] = null;
            });
        }

        [Test]
        public void Indexer_Set_NameNull_ThrowsArgumenNullException()
        {
            var jo = new JsonObject();

            Assert.Throws<ArgumentNullException>(() =>
            {
                jo[null] = 1;
            });
        }

        [Test]
        public void Clone()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var clone = (JsonObject) jo.Clone();

            Assert.That(jo, Is.Not.SameAs(clone));
            Assert.That(jo, Is.EquivalentTo(clone));
        }

        [Test]
        public void Clear()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            jo.Clear();

            Assert.That(jo.Count, Is.EqualTo(0));
            Assert.That(jo, Is.EquivalentTo(new JsonProperty[0]));
        }

        [Test]
        public void ContainsKey()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
            };

            Assert.That(jo.ContainsKey("1"), Is.True);
            Assert.That(jo.ContainsKey("2"), Is.False);
        }

        [Test]
        public void ContainsKey_Null_ThrowsArgumentNullexception()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
            };

            Assert.Throws<ArgumentNullException>(() =>
            {
                jo.ContainsKey(null);
            });
        }

        [Test]
        public void Remove()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
                ["2"] = 2,
            };

            Assert.That(jo.Remove("1"), Is.True);
            Assert.That(jo.Remove("3"), Is.False);

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("2", 2),
            }));
        }
        
        [Test]
        public void Remove_Multiple()
        {
            var jo = new JsonObject {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
                ["4"] = 4,
                ["5"] = 5,
                ["6"] = 6,
                ["7"] = 7,
            };

            Assert.That(jo.Remove("1"), Is.True);
            Assert.That(jo.Remove("7"), Is.True);
            Assert.That(jo.Remove("5"), Is.True);
            Assert.That(jo.Remove("3"), Is.True);

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("2", 2),
                new JsonProperty("4", 4),
                new JsonProperty("6", 6),
            }));
        }
        
        [Test]
        public void Add_Remove_Multiple()
        {
            var jo = new JsonObject();

            jo.Add("1", 1);
            jo.Remove("1");
            
            jo.Add("2", 2);
            jo.Add("3", 3);
            jo.Remove("2");
            
            jo.Add("4", 4);
            jo.Add("5", 5);
            jo.Remove("5");
            
            jo.Add("6", 6);
            jo.Add("7", 7);
            jo.Add("8", 8);
            jo.Remove("7");
            
            jo.Add("9", 9);

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("3", 3),
                new JsonProperty("4", 4),
                new JsonProperty("6", 6),
                new JsonProperty("8", 8),
                new JsonProperty("9", 9),
            }));
        }
        
        [Test]
        public void Add_Remove_Even()
        {
            var jo = new JsonObject {
                { "0", 0 }, 
                { "1", 1 }, 
                { "2", 2 }, 
                { "3", 3 }, 
                { "4", 4 },
                { "5", 5 }, 
                { "6", 6 },
                { "7", 7 },
                { "8", 8 }, 
                { "9", 9 }
            };
            
            jo.Remove("0");
            jo.Remove("2");
            jo.Remove("4");
            jo.Remove("6");
            jo.Remove("8");

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 1),
                new JsonProperty("3", 3),
                new JsonProperty("5", 5),
                new JsonProperty("7", 7),
                new JsonProperty("9", 9),
            }));
        }
        
        [Test]
        public void Add_Remove_Odd()
        {
            var jo = new JsonObject {
                { "0", 0 }, 
                { "1", 1 }, 
                { "2", 2 }, 
                { "3", 3 }, 
                { "4", 4 },
                { "5", 5 }, 
                { "6", 6 },
                { "7", 7 },
                { "8", 8 }, 
                { "9", 9 }
            };
            
            jo.Remove("1");
            jo.Remove("3");
            jo.Remove("5");
            jo.Remove("7");
            jo.Remove("9");

            Assert.That(jo, Is.EquivalentTo(new[]
            {
                new JsonProperty("0", 0),
                new JsonProperty("2", 2),
                new JsonProperty("4", 4),
                new JsonProperty("6", 6),
                new JsonProperty("8", 8),
            }));
        }

        [Test]
        public void Remove_Null_ThrowsArgumentNullException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
            };

            Assert.Throws<ArgumentNullException>(() =>
            {
                jo.Remove(null);
            });
        }

        [Test]
        public void DeepEquals()
        {
            var a = new JsonObject { ["1"] = 1, ["2"] = 2 };

            Assert.That(a.DeepEquals(new JsonObject { ["2"] = 2, ["1"] = 1 }), Is.True);
            Assert.That(a.DeepEquals(new JsonObject { }), Is.False);
            Assert.That(a.DeepEquals(new JsonObject { ["1"] = 1 }), Is.False);
            Assert.That(a.DeepEquals(new JsonObject { ["1"] = 1, ["2"] = 3 }), Is.False);
            Assert.That(a.DeepEquals(new JsonObject { ["1"] = 1, ["3"] = 3 }), Is.False);
            Assert.That(a.DeepEquals(new JsonObject { ["1"] = 1, ["2"] = 2, ["3"] = 3 }), Is.False);
            Assert.That(a.DeepEquals(null), Is.False);
        }

        #region Keys

        [Test]
        public void Keys_IsReadOnly()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.That(dict.Keys.IsReadOnly, Is.True);
        }

        [Test]
        public void Keys_Contains()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            Assert.That(d.Keys.Contains("1"), Is.True);
            Assert.That(d.Keys.Contains("2"), Is.False);
        }

        [Test]
        public void Keys_Contains_Null_ThrowsArgumentNullException()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            Assert.Throws<ArgumentNullException>(() =>
            {
                bool _ = d.Keys.Contains(null);
            });
        }

        [Test]
        public void Keys_Add_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() =>
            {
                dict.Keys.Add("foo");
            });
        }

        [Test]
        public void Keys_Remove_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() =>
            {
                dict.Keys.Remove("foo");
            });
        }

        [Test]
        public void Keys_Clear_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() => dict.Keys.Clear());
        }

        [Test]
        public void Keys_CopyTo_StartIndexOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            var keys = dict.Keys;

            var dst = new string[keys.Count];

            Assert.Throws<ArgumentOutOfRangeException>(() => keys.CopyTo(dst, -1));
        }


        [TestCase(2, 1, 0)]
        public void Keys_CopyTo_NotEnoughSpace_ThrowsArgumentException(int srcLength, int dstLength, int startIndex)
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                dict.Add(i.ToString(), i);
            }

            var dst = new string[dstLength];

            Assert.Throws<ArgumentException>(()
                => dict.Keys.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 1)]
        [TestCase(0, 0, int.MaxValue)]
        [TestCase(2, 1, int.MaxValue)]
        public void Keys_CopyTo_IndexOutOfRange_ThrowsArgumentOutOfRangeException(
            int srcLength, int dstLength, int startIndex)
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                dict.Add(i.ToString(), i);
            }

            var dst = new string[dstLength];

            Assert.Throws<ArgumentOutOfRangeException>(()
                => dict.Keys.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        public void Keys_CopyTo(int srcLength, int dstLength, int startIndex)
        {
            var d = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                d.Add(i.ToString(), i);
            }

            var dst = new string[dstLength];

            d.Keys.CopyTo(dst, startIndex);

            var expectedArray = new string[dstLength];

            for (var i = 0; i < srcLength; i++) {
                expectedArray[i + startIndex] = i.ToString();
            }

            Assert.That(dst, Is.EquivalentTo(expectedArray));
        }

        [Test]
        public void Keys_CopyTo_OutputArrayNull_ThrowsArgumentNullException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<ArgumentNullException>(() => dict.Keys.CopyTo(null, 0));
        }

        [Test]
        public void Keys_GetEnumerator()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            var result = new List<string>();

            foreach (string value in d.Keys) {
                result.Add(value);
            }

            Assert.That(result, Is.EquivalentTo(new[] { "1", "2", "3" }));
        }

        #endregion Keys

        #region Values

        [Test]
        public void Values_IsReadOnly()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.That(dict.Values.IsReadOnly, Is.True);
        }

        [Test]
        public void Values_Add_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() =>
            {
                dict.Values.Add("foo");
            });
        }

        [Test]
        public void Values_Remove_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() =>
            {
                dict.Values.Remove("foo");
            });
        }

        [Test]
        public void Values_Clear_ThrowsNotSupportedException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<NotSupportedException>(() => dict.Values.Clear());
        }

        [Test]
        public void Values_CopyTo_StartIndexOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            var dst = new JsonNode[dict.Values.Count];

            Assert.Throws<ArgumentOutOfRangeException>(() => dict.Values.CopyTo(dst, -1));
        }


        [TestCase(2, 1, 0)]
        public void Values_CopyTo_NotEnoughSpace_ThrowsArgumentException(int srcLength, int dstLength, int startIndex)
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                dict.Add(i.ToString(), i);
            }

            var dst = new JsonNode[dstLength];

            Assert.Throws<ArgumentException>(()
                => dict.Values.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 1)]
        [TestCase(0, 0, int.MaxValue)]
        [TestCase(2, 1, int.MaxValue)]
        public void Values_CopyTo_IndexOutOfRange_ThrowsArgumentOutOfRangeException(
            int srcLength, int dstLength, int startIndex)
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                dict.Add(i.ToString(), i);
            }

            var dst = new JsonNode[dstLength];

            Assert.Throws<ArgumentOutOfRangeException>(()
                => dict.Values.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        public void Values_CopyTo(int srcLength, int dstLength, int startIndex)
        {
            var d = (IDictionary<string, JsonNode>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                d.Add(i.ToString(), i);
            }

            var dst = new JsonNode[dstLength];

            d.Values.CopyTo(dst, startIndex);

            var expectedArray = new JsonNode[dstLength];

            for (var i = 0; i < srcLength; i++) {
                expectedArray[i + startIndex] = i;
            }

            Assert.That(dst, Is.EquivalentTo(expectedArray));
        }

        [Test]
        public void Values_CopyTo_OutputArrayNull_ThrowsArgumentNullException()
        {
            var dict = (IDictionary<string, JsonNode>) new JsonObject();

            Assert.Throws<ArgumentNullException>(() => dict.Values.CopyTo(null, 0));
        }

        [Test]
        public void Values_Contains()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var dict = (IDictionary<string, JsonNode>) jo;

            Assert.That(dict.Values.Contains(1), Is.True);
            Assert.That(dict.Values.Contains(4), Is.False);
        }

        [Test]
        public void Values_Contains_Null_ThrowsArgumentNullException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var dict = (IDictionary<string, JsonNode>) jo;

            Assert.Throws<ArgumentNullException>(() =>
            {
                bool _ = dict.Values.Contains(null);
            });
        }

        [Test]
        public void Values_GetEnumerator()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            var result = new List<JsonNode>();

            foreach (var value in d.Values) {
                result.Add(value);
            }

            Assert.That(result, Is.EquivalentTo(new JsonNode[] { 1, 2, 3 }));
        }

        #endregion Values

        #region ICollection<JsonProperty>

        [Test]
        public void CollectionOfJsonProperty_Add()
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            coll.Add(new JsonProperty("1", 1));
            coll.Add(new JsonProperty("2", 2));

            Assert.That(coll.Count, Is.EqualTo(2));

            Assert.That(coll, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 1),
                new JsonProperty("2", 2),
            }));
        }

        [Test]
        public void CollectionOfJsonProperty_Add_Empty_ThrowsArgumentException()
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            Assert.Throws<ArgumentException>(() =>
                coll.Add(JsonProperty.Empty));
        }

        [Test]
        public void CollectionOfJsonProperty_Add_Contains_ThrowsInvalidOperationException()
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            coll.Add(new JsonProperty("1", 1));

            Assert.Throws<InvalidOperationException>(() =>
                coll.Add(new JsonProperty("1", 1)));
        }

        [Test]
        public void CollectionOfJsonProperty_IsReadOnly()
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            Assert.That(coll.IsReadOnly, Is.False);
        }

        [Test]
        public void CollectionOfJsonProperty_Contains()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<JsonProperty>) jo;

            Assert.That(coll.Contains(new JsonProperty("1", 1)), Is.True);
            Assert.That(coll.Contains(new JsonProperty("1", 2)), Is.False);
            Assert.That(coll.Contains(new JsonProperty("4", 1)), Is.False);
        }

        [Test]
        public void CollectionOfJsonProperty_Contains_Empty_ThrowsArgumentException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<JsonProperty>) jo;

            Assert.Throws<ArgumentException>(() =>
            {
                bool _ = coll.Contains(JsonProperty.Empty);
            });
        }

        [Test]
        public void CollectionOfJsonProperty_Remove()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<JsonProperty>) jo;

            Assert.That(coll.Remove(new JsonProperty("1", 1)), Is.True);

            Assert.That(coll, Is.EquivalentTo(new[]
            {
                new JsonProperty("2", 2),
                new JsonProperty("3", 3),
            }));
        }

        [Test]
        public void CollectionOfJsonProperty_Remove_NotFound()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<JsonProperty>) jo;

            Assert.That(coll.Remove(new JsonProperty("1", 2)), Is.False);
            Assert.That(coll.Remove(new JsonProperty("4", 4)), Is.False);

            Assert.That(coll, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 1),
                new JsonProperty("2", 2),
                new JsonProperty("3", 3),
            }));
        }

        [Test]
        public void CollectionOfJsonProperty_Remove_Empty_ThrowsArgumentException()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<JsonProperty>) jo;

            Assert.Throws<ArgumentException>(() =>
            {
                coll.Remove(JsonProperty.Empty);
            });
        }

        [TestCase(2, 1, 0)]
        public void CollectionOfJsonProperty_CopyTo_NotEnoughSpace_ThrowsArgumentException(
            int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new JsonProperty(i.ToString(), i));
            }

            var dst = new JsonProperty[dstLength];

            Assert.Throws<ArgumentException>(()
                => coll.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 1)]
        [TestCase(0, 0, int.MaxValue)]
        [TestCase(2, 1, int.MaxValue)]
        public void CollectionOfJsonProperty_CopyTo_IndexOutOfRange_ThrowsArgumentOutOfRangeException(
            int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new JsonProperty(i.ToString(), i));
            }

            var dst = new JsonProperty[dstLength];

            Assert.Throws<ArgumentOutOfRangeException>(()
                => coll.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        public void CollectionOfJsonProperty_CopyTo(int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new JsonProperty(i.ToString(), i));
            }

            var dst = new JsonProperty[dstLength];

            coll.CopyTo(dst, startIndex);

            var expectedArray = new JsonProperty[dstLength];

            for (var i = 0; i < srcLength; i++) {
                expectedArray[i + startIndex] = new JsonProperty(i.ToString(), i);
            }

            Assert.That(dst, Is.EquivalentTo(expectedArray));
        }

        [Test]
        public void CollectionOfJsonProperty_CopyTo_OutputArrayNull_ThrowsArgumentNullException()
        {
            var coll = (ICollection<JsonProperty>) new JsonObject();

            Assert.Throws<ArgumentNullException>(() => coll.CopyTo(null, 0));
        }

        #endregion

        #region ICollection<KeyValuePair<String, JsonNode>>

        [Test]
        public void CollectionOfKeyValuePair_IsReadOnly()
        {
            var coll = (ICollection<KeyValuePair<string, JsonNode>>) new JsonObject();

            Assert.That(coll.IsReadOnly, Is.False);
        }


        [TestCase(2, 1, 0)]
        public void CollectionOfKeyValuePair_CopyTo_NotEnoughSpace_ThrowsArgumentException(
            int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<KeyValuePair<string, JsonNode>>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new KeyValuePair<string, JsonNode>(i.ToString(), i));
            }

            var dst = new KeyValuePair<string, JsonNode>[dstLength];

            Assert.Throws<ArgumentException>(()
                => coll.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 1)]
        [TestCase(0, 0, int.MaxValue)]
        [TestCase(2, 1, int.MaxValue)]
        public void CollectionOfKeyValuePair_CopyTo_IndexOutOfRange_ThrowsArgumentOutOfRangeException(
            int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<KeyValuePair<string, JsonNode>>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new KeyValuePair<string, JsonNode>(i.ToString(), i));
            }

            var dst = new KeyValuePair<string, JsonNode>[dstLength];

            Assert.Throws<ArgumentOutOfRangeException>(()
                => coll.CopyTo(dst, startIndex));
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        public void CollectionOfKeyValuePair_CopyTo(int srcLength, int dstLength, int startIndex)
        {
            var coll = (ICollection<KeyValuePair<string, JsonNode>>) new JsonObject();

            for (var i = 0; i < srcLength; i++) {
                coll.Add(new KeyValuePair<string, JsonNode>(i.ToString(), i));
            }

            var dst = new KeyValuePair<string, JsonNode>[dstLength];

            coll.CopyTo(dst, startIndex);

            var expectedArray = new KeyValuePair<string, JsonNode>[dstLength];

            for (var i = 0; i < srcLength; i++) {
                expectedArray[i + startIndex] = new KeyValuePair<string, JsonNode>(i.ToString(), i);
            }

            Assert.That(dst, Is.EquivalentTo(expectedArray));
        }

        [Test]
        public void CollectionOfKeyValuePair_CopyTo_OutputArrayNull_ThrowsArgumentNullException()
        {
            var coll = (ICollection<KeyValuePair<string, JsonNode>>) new JsonObject();

            Assert.Throws<ArgumentNullException>(() => coll.CopyTo(null, 0));
        }

        [Test]
        public void CollectionOfKeyValuePair_Remove()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<KeyValuePair<string, JsonNode>>) jo;

            Assert.That(coll.Remove(new KeyValuePair<string, JsonNode>("1", 1)), Is.True);

            Assert.That(coll, Is.EquivalentTo(new[]
            {
                new JsonProperty("2", 2),
                new JsonProperty("3", 3),
            }));
        }

        [Test]
        public void CollectionOfKeyValuePair_Remove_NotFound()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<KeyValuePair<string, JsonNode>>) jo;

            Assert.That(coll.Remove(new KeyValuePair<string, JsonNode>("1", 2)), Is.False);
            Assert.That(coll.Remove(new KeyValuePair<string, JsonNode>("4", 4)), Is.False);

            Assert.That(coll, Is.EquivalentTo(new[]
            {
                new JsonProperty("1", 1),
                new JsonProperty("2", 2),
                new JsonProperty("3", 3),
            }));
        }

        [Test]
        public void CollectionOfKeyValuePair_Contains()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<KeyValuePair<string, JsonNode>>) jo;

            Assert.That(coll.Contains(new KeyValuePair<string, JsonNode>("1", 1)), Is.True);
            Assert.That(coll.Contains(new KeyValuePair<string, JsonNode>("1", 2)), Is.False);
            Assert.That(coll.Contains(new KeyValuePair<string, JsonNode>("4", 1)), Is.False);
        }

        [Test]
        public void CollectionOfKeyValuePair_GetEnumerator()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            var coll = (ICollection<KeyValuePair<string, JsonNode>>) jo;

            var result = new List<KeyValuePair<string, JsonNode>>();

            foreach (var kvp in coll) {
                result.Add(kvp);
            }

            Assert.That(result, Is.EquivalentTo(new[]
            {
                new KeyValuePair<string, JsonNode>("1", 1),
                new KeyValuePair<string, JsonNode>("2", 2),
                new KeyValuePair<string, JsonNode>("3", 3),
            }));
        }

        #endregion

        #region IDictionary<string, JsonNode>

        [Test]
        public void CollectionOfDictionary_TryGetValue()
        {
            var jo = new JsonObject
            {
                ["1"] = 1,
                ["2"] = 2,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            Assert.That(d.TryGetValue("1", out var v1), Is.True);
            Assert.That(v1, Is.EqualTo((JsonNode) 1));

            Assert.That(d.TryGetValue("3", out var _), Is.False);
        }

        [Test]
        public void CollectionOfDictionary_TryGetValue_Null_ThrowsArgumentNullException()
        {
            var jo = new JsonObject()
            {
                ["1"] = 1,
            };

            var d = (IDictionary<string, JsonNode>) jo;

            Assert.Throws<ArgumentNullException>(() =>
            {
                bool _ = d.TryGetValue(null, out var _);
            });
        }

        #endregion
    }
}
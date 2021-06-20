using System;
using System.Collections.ObjectModel;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public sealed class PrecedenceTests
    {
        private sealed class Converter : JsonConverter
        {
            private string Format { get; } = string.Empty;

            public override string GetString(object value, JsonConverterContext context)
            {
                return context.Format ?? Format;
            }

            public override object GetValue(string s, JsonConverterContext context)
            {
                if (s != (context.Format ?? Format)) {
                    throw new JsonInvalidValueException();
                }

                return Activator.CreateInstance(context.TargetType);
            }
        }

        [JsonConverter(typeof(Converter))]
        private class Base { }

        [JsonFormat("GLOBAL_DEFINED")]
        private class Defined : Base { }

        [JsonFormat("GLOBAL_OVERRIDEN")]
        private class Overriden : Defined { }

        private class DerivedDefined : Defined { }

        private class DerivedBase : Base { }


        [TestCase(typeof(Base), "\"\"")]
        [TestCase(typeof(DerivedBase), "\"\"")]
        [TestCase(typeof(Defined), "\"GLOBAL_DEFINED\"")]
        [TestCase(typeof(Overriden), "\"GLOBAL_OVERRIDEN\"")]
        [TestCase(typeof(DerivedDefined), "\"GLOBAL_DEFINED\"")]
        public void LocalEmpty(Type type, string json)
        {
            var serializer = new JsonSerializer();

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Base), "\"LOCAL_BASE\"")]
        [TestCase(typeof(DerivedBase), "\"LOCAL_BASE\"")]
        [TestCase(typeof(Defined), "\"GLOBAL_DEFINED\"")]
        [TestCase(typeof(Overriden), "\"GLOBAL_OVERRIDEN\"")]
        [TestCase(typeof(DerivedDefined), "\"GLOBAL_DEFINED\"")]
        public void LocalBase(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Base), "\"LOCAL_BASE\"")]
        [TestCase(typeof(DerivedBase), "\"LOCAL_DERIVED_BASE\"")]
        [TestCase(typeof(Defined), "\"GLOBAL_DEFINED\"")]
        [TestCase(typeof(Overriden), "\"GLOBAL_OVERRIDEN\"")]
        [TestCase(typeof(DerivedDefined), "\"GLOBAL_DEFINED\"")]
        public void LocalAll(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            serializer.SetContext<DerivedBase>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_BASE"
            });

            serializer.SetContext<Defined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DEFINED"
            });

            serializer.SetContext<DerivedDefined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_DEFINED"
            });

            serializer.SetContext<Overriden>(new JsonNodeSerializationContext {
                Format = "LOCAL_OVERRIDEN"
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Base), "\"PARAM\"")]
        [TestCase(typeof(DerivedBase), "\"PARAM\"")]
        [TestCase(typeof(Defined), "\"PARAM\"")]
        [TestCase(typeof(DerivedDefined), "\"PARAM\"")]
        [TestCase(typeof(Overriden), "\"PARAM\"")]
        public void LocalAllWithParam(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            serializer.SetContext<DerivedBase>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_BASE"
            });

            serializer.SetContext<Defined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DEFINED"
            });

            serializer.SetContext<DerivedDefined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_DEFINED"
            });

            serializer.SetContext<Overriden>(new JsonNodeSerializationContext {
                Format = "LOCAL_OVERRIDEN"
            });

            var ctx = new JsonNodeSerializationContext() {
                Format = "PARAM"
            };

            var foo = serializer.Deserialize(type, json, ctx);

            var resultJson = serializer.Serialize(type, foo, ctx);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Collection<Base>), "[\"\"]")]
        [TestCase(typeof(Collection<DerivedBase>), "[\"\"]")]
        [TestCase(typeof(Collection<Defined>), "[\"GLOBAL_DEFINED\"]")]
        [TestCase(typeof(Collection<Overriden>), "[\"GLOBAL_OVERRIDEN\"]")]
        [TestCase(typeof(Collection<DerivedDefined>), "[\"GLOBAL_DEFINED\"]")]
        public void Collection_LocalEmpty(Type type, string json)
        {
            var serializer = new JsonSerializer();

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Collection<Base>), "[\"LOCAL_BASE\"]")]
        [TestCase(typeof(Collection<DerivedBase>), "[\"LOCAL_BASE\"]")]
        [TestCase(typeof(Collection<Defined>), "[\"GLOBAL_DEFINED\"]")]
        [TestCase(typeof(Collection<Overriden>), "[\"GLOBAL_OVERRIDEN\"]")]
        [TestCase(typeof(Collection<DerivedDefined>), "[\"GLOBAL_DEFINED\"]")]
        public void Collection_LocalBase(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Collection<Base>), "[\"LOCAL_BASE\"]")]
        [TestCase(typeof(Collection<DerivedBase>), "[\"LOCAL_DERIVED_BASE\"]")]
        [TestCase(typeof(Collection<Defined>), "[\"GLOBAL_DEFINED\"]")]
        [TestCase(typeof(Collection<Overriden>), "[\"GLOBAL_OVERRIDEN\"]")]
        [TestCase(typeof(Collection<DerivedDefined>), "[\"GLOBAL_DEFINED\"]")]
        public void Collection_LocalAll(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            serializer.SetContext<DerivedBase>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_BASE"
            });

            serializer.SetContext<Defined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DEFINED"
            });

            serializer.SetContext<DerivedDefined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_DEFINED"
            });

            serializer.SetContext<Overriden>(new JsonNodeSerializationContext {
                Format = "LOCAL_OVERRIDEN"
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Collection<Base>), "[\"LOCAL_BASE_CHILD\"]")]
        [TestCase(typeof(Collection<DerivedBase>), "[\"LOCAL_DERIVED_BASE\"]")]
        [TestCase(typeof(Collection<Defined>), "[\"GLOBAL_DEFINED\"]")]
        [TestCase(typeof(Collection<Overriden>), "[\"GLOBAL_OVERRIDEN\"]")]
        [TestCase(typeof(Collection<DerivedDefined>), "[\"GLOBAL_DEFINED\"]")]
        public void Collection_LocalAll_LocalCollBase(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            serializer.SetContext<DerivedBase>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_BASE"
            });

            serializer.SetContext<Defined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DEFINED"
            });

            serializer.SetContext<DerivedDefined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_DEFINED"
            });

            serializer.SetContext<Overriden>(new JsonNodeSerializationContext {
                Format = "LOCAL_OVERRIDEN"
            });

            serializer.SetContext<Collection<Base>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_BASE_CHILD"
                }
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        [TestCase(typeof(Collection<Base>), "[\"LOCAL_BASE_CHILD\"]")]
        [TestCase(typeof(Collection<DerivedBase>), "[\"LOCAL_DERIVED_BASE_CHILD\"]")]
        [TestCase(typeof(Collection<Defined>), "[\"LOCAL_DEFINED_CHILD\"]")]
        [TestCase(typeof(Collection<Overriden>), "[\"LOCAL_OVERRIDEN_CHILD\"]")]
        [TestCase(typeof(Collection<DerivedDefined>), "[\"LOCAL_DERIVED_DEFINED_CHILD\"]")]
        public void Collection_LocalAll_LocalCollAll(Type type, string json)
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Base>(new JsonNodeSerializationContext {
                Format = "LOCAL_BASE"
            });

            serializer.SetContext<DerivedBase>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_BASE"
            });

            serializer.SetContext<Defined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DEFINED"
            });

            serializer.SetContext<DerivedDefined>(new JsonNodeSerializationContext {
                Format = "LOCAL_DERIVED_DEFINED"
            });

            serializer.SetContext<Overriden>(new JsonNodeSerializationContext {
                Format = "LOCAL_OVERRIDEN"
            });

            serializer.SetContext<Collection<Base>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_BASE_CHILD"
                }
            });

            serializer.SetContext<Collection<DerivedBase>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_DERIVED_BASE_CHILD"
                }
            });

            serializer.SetContext<Collection<Defined>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_DEFINED_CHILD"
                }
            });

            serializer.SetContext<Collection<DerivedDefined>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_DERIVED_DEFINED_CHILD"
                }
            });

            serializer.SetContext<Collection<Overriden>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext {
                    Format = "LOCAL_OVERRIDEN_CHILD"
                }
            });

            var foo = serializer.Deserialize(type, json);

            var resultJson = serializer.Serialize(type, foo);

            Assert.That(resultJson, Is.EqualTo(json));
        }

        // [Test]
        // public void Collection_Global_Format()
        // {
        //     var json = "[\"GLOBAL_BAR_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     var coll = serializer.Deserialize<List<NoFormat>>(json);
        //
        //     var resultJson = serializer.Serialize(coll);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Global_Child_Format()
        // {
        //     var json = "[\"GLOBAL_CHILD_BAR_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     var coll = serializer.Deserialize<BarCollection>(json);
        //
        //     var resultJson = serializer.Serialize(coll);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Local_Base_Format()
        // {
        //     var json = "[\"LOCAL_BASE_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     serializer.ConfigureContextFor<B>(c =>
        //         c.Format = "LOCAL_BASE_FMT");
        //
        //     var coll = serializer.Deserialize<BarCollection>(json);
        //
        //     var resultJson = serializer.Serialize(coll);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Local_Format()
        // {
        //     var json = "[\"LOCAL_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     serializer.ConfigureContextFor<B>(c =>
        //         c.Format = "LOCAL_BASE_FMT");
        //
        //     serializer.ConfigureContextFor<NoFormat>(c =>
        //         c.Format = "LOCAL_FMT");
        //
        //     var list = serializer.Deserialize<BarCollection>(json);
        //
        //     var resultJson = serializer.Serialize(list);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Local_Child_Format()
        // {
        //     var json = "[\"LOCAL_CHILD_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     serializer.ConfigureContextFor<B>(c =>
        //         c.Format = "LOCAL_BASE_FMT");
        //
        //     serializer.ConfigureContextFor<NoFormat>(c =>
        //         c.Format = "LOCAL_FMT");
        //
        //     serializer.ConfigureContextFor<BarCollection>(c =>
        //         c.ChildContext = new JsonSerializationContext() {
        //             Format = "LOCAL_CHILD_FMT"
        //         });
        //
        //     var list = serializer.Deserialize<BarCollection>(json);
        //
        //     var resultJson = serializer.Serialize(list);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Local_Child_Base_Format()
        // {
        //     var json = "[\"LOCAL_CHILD_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     serializer.ConfigureContextFor<B>(c =>
        //         c.Format = "LOCAL_BASE_FMT");
        //
        //     serializer.ConfigureContextFor<NoFormat>(c =>
        //         c.Format = "LOCAL_FMT");
        //
        //     serializer.ConfigureContextFor<BarCollection>(c =>
        //         c.ChildContext = new JsonSerializationContext() {
        //             Format = "LOCAL_CHILD_FMT"
        //         });
        //
        //     serializer.ConfigureContextFor<Collection<NoFormat>>(c =>
        //         c.ChildContext = new JsonSerializationContext() {
        //             Format = "LOCAL_CHILD_BASE_FMT"
        //         });
        //
        //     var list = serializer.Deserialize<BarCollection>(json);
        //
        //     var resultJson = serializer.Serialize(list);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
        //
        // [Test]
        // public void Collection_Param_Format()
        // {
        //     var json = "[\"PARAM_CHILD_FMT\"]";
        //
        //     var serializer = new JsonSerializer();
        //
        //     serializer.ConfigureContextFor<B>(c =>
        //         c.Format = "LOCAL_BASE_FMT");
        //
        //     serializer.ConfigureContextFor<NoFormat>(c =>
        //         c.Format = "LOCAL_FMT");
        //
        //     serializer.ConfigureContextFor<BarCollection>(c =>
        //         c.ChildContext = new JsonSerializationContext() {
        //             Format = "LOCAL_CHILD_FMT"
        //         });
        //
        //     serializer.ConfigureContextFor<Collection<NoFormat>>(c =>
        //         c.ChildContext = new JsonSerializationContext() {
        //             Format = "LOCAL_CHILD_BASE_FMT"
        //         });
        //
        //     var ctx = new JsonSerializationContext {
        //         ChildContext = new JsonSerializationContext {
        //             Format = "PARAM_CHILD_FMT"
        //         }
        //     };
        //
        //     var list = serializer.Deserialize<BarCollection>(json, ctx);
        //
        //     var resultJson = serializer.Serialize(list, ctx);
        //
        //     Assert.That(resultJson, Is.EqualTo(json));
        // }
    }
}
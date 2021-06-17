using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Functional
{
    public sealed class JsonSerializationTests
    {
        private sealed class OrderedDictionary<TKey, TValue> : Collection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
        {
            void IDictionary<TKey, TValue>.Add(TKey key, TValue value) 
                => Add(new KeyValuePair<TKey, TValue>(key, value));
            
            bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => throw new NotSupportedException();
            bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new NotSupportedException();
            bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => throw new NotSupportedException();

            TValue IDictionary<TKey, TValue>.this[TKey key]
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            ICollection<TKey> IDictionary<TKey, TValue>.Keys => throw new NotSupportedException();
            ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new NotSupportedException();
        }
        
        private static JsonReader CreateReader(string json, Type readerType)
        {
            if (readerType == typeof(JsonStringReader)) {
                return new JsonStringReader(json);
            }

            if (readerType == typeof(JsonStreamReader)) {

                var ms = new MemoryStream();

                using (var sw = new StreamWriter(ms, null, -1, true)) {
                    sw.Write(json);
                }

                ms.Position = 0;

                return new JsonStreamReader(ms) {
                    CloseInput = true
                };
            }

            if (readerType == typeof(JsonTextReader)) {
                return new JsonTextReader(new StringReader(json)) {
                    CloseInput = true
                };
            }

            if (readerType == typeof(JsonNodeReader)) {
                return new JsonNodeReader(JsonNode.Parse(json));
            }

            throw new ArgumentOutOfRangeException(nameof(readerType));
        }

        #region Meteorite Landing

        private sealed class MeteoriteLanding
        {
            [JsonNode("name")]
            public string Name { get; set; }

            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonNode("id")]
            public int ID { get; set; }

            [JsonNode("nametype")]
            public string NameType { get; set; }

            [JsonNode("recclass")]
            public string RecClass { get; set; }

            [JsonOptional]
            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonNode("mass")]
            public decimal? Mass { get; set; }

            [JsonNode("fall")]
            public string Fall { get; set; }

            [JsonOptional]
            [JsonFormat("yyyy-MM-ddTHH:mm:ss.fff")]
            [JsonNode("year")]
            [JsonConverter(typeof(JsonDateTimeConverter))]
            public DateTime? Year { get; set; }

            [JsonOptional]
            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonFormat(".6")]
            [JsonNode("reclat")]
            public double? RecLat { get; set; }

            [JsonOptional]
            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonFormat(".6")]
            [JsonNode("reclong")]
            public double? RecLong { get; set; }

            [JsonOptional]
            [JsonTypePropertyName("type")]
            [JsonNode("geolocation")]
            public MeteoriteLandingGeolocation Geolocation { get; set; }

            [JsonOptional]
            [JsonNode(":@computed_region_cbhk_fwbd")]
            public string ComputedRegionCbhkFwbd { get; set; }

            [JsonOptional]
            [JsonNode(":@computed_region_nnqa_25f4")]
            public string ComputedRegionNnqa25f4 { get; set; }
        }

        private abstract class MeteoriteLandingGeolocation { }

        private sealed class MeteoriteLandingPointGeolocation : MeteoriteLandingGeolocation
        {
            [JsonNode("coordinates")]
            [JsonFormat(".0", ApplyTo = JsonNodeTarget.Item)]
            public List<double> Coordinates { get; set; }
        }


        [TestCase(typeof(List<MeteoriteLanding>), typeof(JsonStringReader))]
        [TestCase(typeof(List<MeteoriteLanding>), typeof(JsonStreamReader))]
        [TestCase(typeof(List<MeteoriteLanding>), typeof(JsonTextReader))]
        [TestCase(typeof(List<MeteoriteLanding>), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_Meteorite(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("Earth_Meteorite_Landings.min.json");

            var serializer = new JsonSerializer();

            serializer.RegisterType<MeteoriteLandingPointGeolocation>("Point");

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region VAT Rates

        private enum VatRatesCountry
        {
            [JsonEnumValue("HU")] Hungary,
            [JsonEnumValue("ES")] Spain,
            [JsonEnumValue("BG")] Bulgaria,
            [JsonEnumValue("LV")] Latvia,
            [JsonEnumValue("PL")] Poland,
            [JsonEnumValue("UK")] UnitedKingdom,
            [JsonEnumValue("CZ")] CzechRepublic,
            [JsonEnumValue("MT")] Malta,
            [JsonEnumValue("IT")] Italy,
            [JsonEnumValue("SI")] Slovenia,
            [JsonEnumValue("IE")] Ireland,
            [JsonEnumValue("SE")] Sweden,
            [JsonEnumValue("DK")] Denmark,
            [JsonEnumValue("FI")] Finland,
            [JsonEnumValue("CY")] Cyprus,
            [JsonEnumValue("LU")] Luxembourg,
            [JsonEnumValue("RO")] Romania,
            [JsonEnumValue("EE")] Estonia,
            [JsonEnumValue("EL")] Greece,
            [JsonEnumValue("LT")] Lithuania,
            [JsonEnumValue("FR")] France,
            [JsonEnumValue("HR")] Croatia,
            [JsonEnumValue("BE")] Belgium,
            [JsonEnumValue("NL")] Netherlands,
            [JsonEnumValue("SK")] Slovakia,
            [JsonEnumValue("DE")] Germany,
            [JsonEnumValue("PT")] Portugal,
            [JsonEnumValue("AT")] Austria,
        }

        private sealed class VatRates
        {
            [JsonNode("details")]
            [JsonConverter(typeof(JsonUriConverter))]
            public Uri Details { get; set; }

            [JsonNode("version"), JsonOptional, JsonSerializeNull]
            public string Version { get; set; }

            [JsonNode("rates")]
            public List<VatRatesItem> Rates { get; set; }
        }

        private sealed class VatRatesItem
        {
            [JsonNode("name")]
            public string Name { get; set; }

            [JsonNode("code")]
            public VatRatesCountry Code { get; set; }

            [JsonNode("country_code")]
            public string CountryCode { get; set; }

            [JsonNode("periods")]
            public List<VatRatesItemPeriod> Periods { get; set; }
        }

        private sealed class VatRatesItemPeriod
        {
            [JsonNode("effective_from")]
            public string EffectiveFrom { get; set; }

            [JsonFormat(".1", ApplyTo = JsonNodeTarget.Item)]
            [JsonNode("rates")]
            public OrderedDictionary<string, decimal> Rates { get; set; }
        }

        [TestCase(typeof(VatRates), typeof(JsonStringReader))]
        [TestCase(typeof(VatRates), typeof(JsonStreamReader))]
        [TestCase(typeof(VatRates), typeof(JsonTextReader))]
        [TestCase(typeof(VatRates), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_VatRates(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("VAT_Rates_for_EU.min.json");

            var serializer = new JsonSerializer();

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region Exchange Rates

        private enum ExchangeRatesCurrency
        {
            [JsonEnumValue] USD,
            [JsonEnumValue] AED,
            [JsonEnumValue] ARS,
            [JsonEnumValue] AUD,
            [JsonEnumValue] BGN,
            [JsonEnumValue] BRL,
            [JsonEnumValue] BSD,
            [JsonEnumValue] CAD,
            [JsonEnumValue] CHF,
            [JsonEnumValue] CLP,
            [JsonEnumValue] CNY,
            [JsonEnumValue] COP,
            [JsonEnumValue] CZK,
            [JsonEnumValue] DKK,
            [JsonEnumValue] DOP,
            [JsonEnumValue] EGP,
            [JsonEnumValue] EUR,
            [JsonEnumValue] FJD,
            [JsonEnumValue] GBP,
            [JsonEnumValue] GTQ,
            [JsonEnumValue] HKD,
            [JsonEnumValue] HRK,
            [JsonEnumValue] HUF,
            [JsonEnumValue] IDR,
            [JsonEnumValue] ILS,
            [JsonEnumValue] INR,
            [JsonEnumValue] ISK,
            [JsonEnumValue] JPY,
            [JsonEnumValue] KRW,
            [JsonEnumValue] KZT,
            [JsonEnumValue] MXN,
            [JsonEnumValue] MYR,
            [JsonEnumValue] NOK,
            [JsonEnumValue] NZD,
            [JsonEnumValue] PAB,
            [JsonEnumValue] PEN,
            [JsonEnumValue] PHP,
            [JsonEnumValue] PKR,
            [JsonEnumValue] PLN,
            [JsonEnumValue] PYG,
            [JsonEnumValue] RON,
            [JsonEnumValue] RUB,
            [JsonEnumValue] SAR,
            [JsonEnumValue] SEK,
            [JsonEnumValue] SGD,
            [JsonEnumValue] THB,
            [JsonEnumValue] TRY,
            [JsonEnumValue] TWD,
            [JsonEnumValue] UAH,
            [JsonEnumValue] UYU,
            [JsonEnumValue] ZAR,
        }

        private sealed class ExchangeRates
        {
            [JsonNode("base")]
            public ExchangeRatesCurrency Base { get; set; }

            [JsonNode("date")]
            [JsonFormat("yyyy-MM-dd")]
            [JsonConverter(typeof(JsonDateTimeConverter))]
            public DateTime Date { get; set; }

            [JsonNode("time_last_updated")]
            public int TimeLastUpdated { get; set; }

            [JsonNode("rates")]
            public SortedDictionary<ExchangeRatesCurrency, decimal> Rates { get; set; }
        }

        [TestCase(typeof(ExchangeRates), typeof(JsonStringReader))]
        [TestCase(typeof(ExchangeRates), typeof(JsonStreamReader))]
        [TestCase(typeof(ExchangeRates), typeof(JsonTextReader))]
        [TestCase(typeof(ExchangeRates), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_ExchangeRates(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("Exchange_Rate_USD.min.json");

            var serializer = new JsonSerializer();

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region Global Temperature Anomaly

        private sealed class GlobalTemperatureAnomaly
        {
            [JsonNode("description")]
            public GlobalTemperatureAnomalyDescription Description { get; set; }

            [JsonNode("data")]
            [JsonConverter(typeof(JsonNumberConverter), ApplyTo = JsonNodeTarget.Item)]
            [JsonConverter(typeof(JsonNumberConverter), ApplyTo = JsonNodeTarget.Key)]
            [JsonFormat(".2", ApplyTo = JsonNodeTarget.Item)]
            public OrderedDictionary<int, double> Data { get; set; }
        }

        private sealed class GlobalTemperatureAnomalyDescription
        {
            [JsonNode("title")]
            public string Title { get; set; }

            [JsonNode("units")]
            public string Units { get; set; }

            [JsonNode("base_period")]
            public string BasePeriod { get; set; }

            [JsonNode("missing")]
            public int Missing { get; set; }
        }

        [TestCase(typeof(GlobalTemperatureAnomaly), typeof(JsonStringReader))]
        [TestCase(typeof(GlobalTemperatureAnomaly), typeof(JsonStreamReader))]
        [TestCase(typeof(GlobalTemperatureAnomaly), typeof(JsonTextReader))]
        [TestCase(typeof(GlobalTemperatureAnomaly), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_GlobalTemperatureAnomaly(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("Global_Temperature_Anomaly.min.json");

            var serializer = new JsonSerializer();

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region Annual Temperature Anomaly

        private sealed class AnnualTemperatureAnomaly
        {
            [JsonNode("description")]
            public AnnualTemperatureAnomalyDescription Description { get; set; }

            [JsonNode("data")]
            [JsonConverter(typeof(JsonDateTimeConverter), ApplyTo = JsonNodeTarget.Key)]
            [JsonFormat("yyyyMM", ApplyTo = JsonNodeTarget.Key)]
            public OrderedDictionary<DateTime, AnnualTemperatureAnomalyData> Data { get; set; }
        }

        private sealed class AnnualTemperatureAnomalyDescription
        {
            [JsonNode("title")]
            public string Title { get; set; }

            [JsonNode("units")]
            public string Units { get; set; }

            [JsonNode("base_period")]
            public string BasePeriod { get; set; }

            [JsonNode("missing")]
            [JsonConverter(typeof(JsonNumberConverter))]
            public int Missing { get; set; }
        }

        private struct AnnualTemperatureAnomalyData
        {
            [JsonNode("value")]
            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonFormat(".2")]
            public double Value { get; set; }

            [JsonNode("anomaly")]
            [JsonConverter(typeof(JsonNumberConverter))]
            [JsonFormat(".2")]
            public double Anomaly { get; set; }
        }

        [TestCase(typeof(AnnualTemperatureAnomaly), typeof(JsonStringReader))]
        [TestCase(typeof(AnnualTemperatureAnomaly), typeof(JsonStreamReader))]
        [TestCase(typeof(AnnualTemperatureAnomaly), typeof(JsonTextReader))]
        [TestCase(typeof(AnnualTemperatureAnomaly), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_AnnualTemperatureAnomaly(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("US_Annual_Average_Temperature_and_Anomaly.min.json");

            var serializer = new JsonSerializer();

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region Earthquakes

        private sealed class EarthquakesFeatureCollection
        {
            [JsonNode("type")]
            public string Type { get; set; }

            [JsonNode("metadata")]
            public EarthquakesMetadata Metadata { get; set; }

            [JsonNode("features")]
            public List<EarthquakesFeature> Features { get; set; }

            [JsonNode("bbox")]
            public List<double> Bbox { get; set; }
        }

        private sealed class EarthquakesMetadata
        {
            [JsonNode("generated")]
            public long Generated { get; set; }

            [JsonNode("url")]
            [JsonConverter(typeof(JsonUriConverter))]
            public Uri Url { get; set; }

            [JsonNode("title")]
            public string Title { get; set; }

            [JsonNode("status")]
            public int Status { get; set; }

            [JsonNode("api")]
            [JsonConverter(typeof(JsonVersionConverter))]
            public Version Api { get; set; }

            [JsonNode("count")]
            public int Count { get; set; }
        }

        private sealed class EarthquakesFeature
        {
            [JsonNode("type")]
            public string Type { get; set; }

            [JsonNode("properties")]
            public EarthquakesProperties Properties { get; set; }

            [JsonNode("geometry")]
            public EarthquakesPoint Geometry { get; set; }

            [JsonNode("id")]
            public string Id { get; set; }
        }

        private sealed class EarthquakesProperties
        {
            [JsonNode("mag")]
            public double Mag { get; set; }

            [JsonNode("place")]
            public string Place { get; set; }

            [JsonNode("time")]
            public long Time { get; set; }

            [JsonNode("updated")]
            public long Updated { get; set; }

            [JsonNode("tz"), JsonOptional]
            public string Tz { get; set; }

            [JsonNode("url")]
            [JsonConverter(typeof(JsonUriConverter))]
            public Uri Url { get; set; }

            [JsonNode("detail")]
            [JsonConverter(typeof(JsonUriConverter))]
            public Uri Detail { get; set; }

            [JsonNode("felt"), JsonOptional]
            public int? Felt { get; set; }

            [JsonNode("cdi"), JsonOptional]
            public double? Cdi { get; set; }

            [JsonNode("mmi"), JsonOptional]
            public double? Mmi { get; set; }

            [JsonNode("alert"), JsonOptional]
            public string Alert { get; set; }

            [JsonNode("status")]
            public string Status { get; set; }

            [JsonNode("tsunami")]
            public int Tsunami { get; set; }

            [JsonNode("sig")]
            public int Sig { get; set; }

            [JsonNode("net")]
            public string Net { get; set; }

            [JsonNode("code")]
            public string Code { get; set; }

            [JsonNode("ids")]
            public string[] Ids { get; set; }

            [JsonNode("sources")]
            public string[] Sources { get; set; }

            [JsonNode("types")]
            public string[] Types { get; set; }

            [JsonNode("nst"), JsonOptional]
            public int? Nst { get; set; }

            [JsonNode("dmin"), JsonOptional]
            public double? Dmin { get; set; }

            [JsonNode("rms")]
            public double Rms { get; set; }

            [JsonNode("gap"), JsonOptional]
            public double? Gap { get; set; }

            [JsonNode("magType")]
            public string MagType { get; set; }

            [JsonNode("type")]
            public string Type { get; set; }

            [JsonNode("title")]
            public string Title { get; set; }
        }

        private sealed class EarthquakesPoint
        {
            [JsonNode("type")]
            public string Type { get; set; }

            [JsonNode("coordinates")]
            public List<double> Coordinates { get; set; }
        }

        private sealed class StringCollectionConverter : JsonStringConverter
        {
            public override string GetString(object value, JsonConverterContext context)
            {
                return string.Join(",", (string[]) value);
            }

            public override object GetValue(string s, JsonConverterContext context)
            {
                return s.Split(",");
            }
        }

        [TestCase(typeof(EarthquakesFeatureCollection), typeof(JsonStringReader))]
        [TestCase(typeof(EarthquakesFeatureCollection), typeof(JsonStreamReader))]
        [TestCase(typeof(EarthquakesFeatureCollection), typeof(JsonTextReader))]
        [TestCase(typeof(EarthquakesFeatureCollection), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_Earthquakes(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("Earthquakes.min.json");

            var serializer = new JsonSerializer {
                SerializeNullProperty = true
            };

            serializer.SetContext<double>(new JsonNodeSerializationContext {
                Format = ".0"
            });
            
            serializer.SetContext<string[]>(new JsonNodeSerializationContext {
                Converter = new StringCollectionConverter()
            });

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            serializer.Serialize(type, jw, obj);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        #endregion

        #region Nobel Prizes

        private sealed class NobelPrizes
        {
            [JsonNode("prizes")]
            public List<NobelPrizesItem> Prizes { get; set; }
        }

        private sealed class NobelPrizesItem
        {
            [JsonNode("year")]
            [JsonConverter(typeof(JsonNumberConverter))]
            public int Year { get; set; }

            [JsonNode("category")]
            public string Category { get; set; }

            [JsonNode("overallMotivation"), JsonOptional]
            public string OverallMotivation { get; set; }

            [JsonNode("laureates"), JsonOptional]
            public List<NobelPrizesLaureate> Laureates { get; set; }
        }

        private sealed class NobelPrizesLaureate
        {
            [JsonNode("id")]
            [JsonConverter(typeof(JsonNumberConverter))]
            public int ID { get; set; }

            [JsonNode("firstname")]
            public string Firstname { get; set; }

            [JsonNode("surname"), JsonOptional]
            public string Surname { get; set; }

            [JsonNode("motivation")]
            public string Motivation { get; set; }

            [JsonNode("share")]
            [JsonConverter(typeof(JsonNumberConverter))]
            public int Share { get; set; }
        }

        [TestCase(typeof(NobelPrizes), typeof(JsonStringReader))]
        [TestCase(typeof(NobelPrizes), typeof(JsonStreamReader))]
        [TestCase(typeof(NobelPrizes), typeof(JsonTextReader))]
        [TestCase(typeof(NobelPrizes), typeof(JsonNodeReader))]
        public void DeserializeAndSerialize_NobelPrizes(Type type, Type readerType)
        {
            var json = JsonSamples.GetJsonAsString("Nobel_Prizes.min.json");

            var serializer = new JsonSerializer();

            using var jr = CreateReader(json, readerType);

            var obj = serializer.Deserialize(type, jr);

            using var sw = new StringWriter();
            using var jw = new JsonNodeWriter();

            serializer.Serialize(type, jw, obj);

            Assert.That(JsonNode.Parse(json).DeepEquals(jw.GetNode()), Is.True);
        }

        #endregion
    }
}
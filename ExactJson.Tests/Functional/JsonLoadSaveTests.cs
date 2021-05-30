using System.IO;

using NUnit.Framework;

namespace ExactJson.Tests.Functional
{
    public sealed class JsonLoadSaveTests
    {
        [TestCase("Global_Temperature_Anomaly.min.json", false, false)]
        [TestCase("US_Annual_Average_Temperature_and_Anomaly.min.json", false, false)]
        [TestCase("VAT_Rates_for_EU.min.json", false, false)]
        [TestCase("Exchange_Rate_USD.min.json", false, false)]
        [TestCase("Earthquakes.min.json", false, false)]
        [TestCase("Nobel_Prizes.min.json", true, true)]
        [TestCase("Reddit_All_Feed.min.json", false, true)]
        [TestCase("Earth_Meteorite_Landings.min.json", false, false)]
        public void LoadAndSave_MinifiedFiles(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonData.GetJsonAsString(fileName);

            using var jr = new JsonStringReader(json);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
            };

            JsonNode.Load(jr).WriteTo(jw);

            var result = sw.ToString();

            Assert.That(result, Is.EqualTo(json));
        }

        [TestCase("Global_Temperature_Anomaly.json", false, false)]
        [TestCase("US_Annual_Average_Temperature_and_Anomaly.json", false, false)]
        [TestCase("VAT_Rates_for_EU.json", false, false)]
        [TestCase("Exchange_Rate_USD.json", false, false)]
        [TestCase("Earthquakes.json", false, false)]
        [TestCase("Nobel_Prizes.json", true, true)]
        [TestCase("Reddit_All_Feed.json", false, true)]
        [TestCase("Earth_Meteorite_Landings.json", false, false)]
        public void LoadAndSave_FormattedFiles(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonData.GetJsonAsString(fileName).Replace("\r", "");

            using var jr = new JsonStringReader(json);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
                Formatted = true,
            };

            JsonNode.Load(jr).WriteTo(jw);

            var result = sw.ToString();

            Assert.That(result, Is.EqualTo(json));
        }

        [TestCase("Global_Temperature_Anomaly.min.json", false, false)]
        [TestCase("US_Annual_Average_Temperature_and_Anomaly.min.json", false, false)]
        [TestCase("VAT_Rates_for_EU.min.json", false, false)]
        [TestCase("Exchange_Rate_USD.min.json", false, false)]
        [TestCase("Earthquakes.min.json", false, false)]
        [TestCase("Nobel_Prizes.min.json", true, true)]
        [TestCase("Reddit_All_Feed.min.json", false, true)]
        [TestCase("Earth_Meteorite_Landings.min.json", false, false)]
        public void LoadAndSave_ObjectReader_MinifiedFiles(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonData.GetJsonAsString(fileName);

            using var jr = new JsonStringReader(json);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
            };

            var tree = JsonNode.Load(jr);

            JsonNode.Load(new JsonNodeReader(tree)).WriteTo(jw);

            var result = sw.ToString();

            Assert.That(result, Is.EqualTo(json));
        }
    }
}
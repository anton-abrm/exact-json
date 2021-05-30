using NUnit.Framework;

namespace ExactJson.Tests.Functional
{
    public sealed class JsonDiffTests
    {
        [TestCase("Global_Temperature_Anomaly.min.json")]
        [TestCase("US_Annual_Average_Temperature_and_Anomaly.min.json")]
        [TestCase("VAT_Rates_for_EU.min.json")]
        [TestCase("Exchange_Rate_USD.min.json")]
        [TestCase("Earthquakes.min.json")]
        [TestCase("Nobel_Prizes.min.json")]
        [TestCase("Reddit_All_Feed.min.json")]
        [TestCase("Earth_Meteorite_Landings.min.json")]
        public void Diff_FormattedFiles(string fileName)
        {
            string json = JsonSamples.GetJsonAsString(fileName);

            var node1 = JsonNode.Parse(json);
            var node2 = JsonNode.Parse(json);

            var diff = node1.Diff(node2);

            Assert.That(diff.Length, Is.EqualTo(0));
        }
    }
}
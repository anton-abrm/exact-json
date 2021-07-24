// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

using NUnit.Framework;

namespace ExactJson.Tests.Functional
{
    public class JsonCopyTests
    {
        private static readonly object[] Files =
        {
            new object[] { "Global_Temperature_Anomaly.json", false, false },
            new object[] { "Global_Temperature_Anomaly.min.json", false, false },
            new object[] { "US_Annual_Average_Temperature_and_Anomaly.json", false, false },
            new object[] { "US_Annual_Average_Temperature_and_Anomaly.min.json", false, false },
            new object[] { "VAT_Rates_for_EU.json", false, false },
            new object[] { "VAT_Rates_for_EU.min.json", false, false },
            new object[] { "Exchange_Rate_USD.json", false, false },
            new object[] { "Exchange_Rate_USD.min.json", false, false },
            new object[] { "Earthquakes.json", false, false },
            new object[] { "Earthquakes.min.json", false, false },
            new object[] { "Nobel_Prizes.json", true, true },
            new object[] { "Nobel_Prizes.min.json", true, true },
            new object[] { "Reddit_All_Feed.json", false, true },
            new object[] { "Reddit_All_Feed.min.json", false, true },
            new object[] { "Earth_Meteorite_Landings.json", false, false },
            new object[] { "Earth_Meteorite_Landings.min.json", false, false },
        };

        [TestCaseSource(nameof(Files))]
        public void CopyTo_String(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonSamples.GetJsonAsString(fileName);

            using var jr = new JsonStringReader(json);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
                Formatted = true,
            };

            if (fileName.Contains(".min.")) {
                jw.Formatted = false;
            }

            jr.CopyTo(jw);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        [TestCaseSource(nameof(Files))]
        public void CopyTo_Stream(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonSamples.GetJsonAsString(fileName);

            using var stream = JsonSamples.GetJsonAsStream(fileName);
            using var jr = new JsonStreamReader(stream);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
                Formatted = true,
            };

            if (fileName.Contains(".min.")) {
                jw.Formatted = false;
            }

            jr.CopyTo(jw);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }

        [TestCaseSource(nameof(Files))]
        public void CopyTo_TextReader(
            string fileName,
            bool escapeSolidus,
            bool escapeNonAscii)
        {
            string json = JsonSamples.GetJsonAsString(fileName);

            using var stream = JsonSamples.GetJsonAsStream(fileName);
            using var sr = new StreamReader(stream);
            using var jr = new JsonTextReader(sr);
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = escapeNonAscii,
                WriteHexInLowerCase = true,
                EscapeSolidus = escapeSolidus,
                Formatted = true,
            };

            if (fileName.Contains(".min.")) {
                jw.Formatted = false;
            }

            jr.CopyTo(jw);

            Assert.That(sw.ToString(), Is.EqualTo(json.Replace("\r", "")));
        }
    }
}
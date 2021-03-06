// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ExactJson.Tests.Functional
{
    internal static class JsonData
    {
        public static string GetJsonAsString(string fileName)
        {
            using (var reader = new StreamReader(GetJsonAsStream(fileName), Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetJsonAsStream(string fileName)
        {
            var resourceName = $"ExactJson.Tests.Data.{fileName}";

            var stream = Assembly
                        .GetExecutingAssembly()
                        .GetManifestResourceStream(resourceName);

            if (stream is null) {
                throw new InvalidOperationException($"Resource '{resourceName}' not found.");
            }

            return stream;
        }
    }
}
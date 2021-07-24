// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExactJson.Infra
{
    internal static class HexUtil
    {
        private const string HexLower = "0123456789abcdef";
        private const string HexUpper = "0123456789ABCDEF";

        public static byte[] FromHex(string hex)
        {
            if (hex is null)
                throw new ArgumentNullException(nameof(hex));

            if (hex.Length % 2 != 0)
                throw new FormatException(
                    "Invalid length for a Base-16 string.");

            var bytes = new byte[hex.Length / 2];

            for (var i = 0; i < bytes.Length; i++) {
                bytes[i] |= (byte) (GetIndex(hex[i * 2 + 0]) << 4);
                bytes[i] |= (byte) (GetIndex(hex[i * 2 + 1]) << 0);
            }

            return bytes;

            static int GetIndex(char ch) => ch switch {
                >= '0' and <= '9' => ch - '0' + 0x00,
                >= 'a' and <= 'f' => ch - 'a' + 0x0A,
                >= 'A' and <= 'F' => ch - 'A' + 0x0A,
                _ => throw new FormatException(
                    "The input is not a valid Base-16 string.")
            };
        }

        public static string ToHex(byte[] bytes, bool upperCase = false)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var alphabet = upperCase
                ? HexUpper
                : HexLower;

            var chars = new char[bytes.Length * 2];

            for (var i = 0; i < bytes.Length; i++) {
                chars[i * 2 + 0] = alphabet[(bytes[i] & 0xF0) >> 4];
                chars[i * 2 + 1] = alphabet[(bytes[i] & 0x0F) >> 0];
            }

            return new string(chars);
        }

        public static bool IsHex(char ch) 
            => ch is >= '0' and <= '9' or
                     >= 'a' and <= 'f' or
                     >= 'A' and <= 'F';
    }

}
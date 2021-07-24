﻿// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    public class HexUtilTests
    {
        private static readonly char[] LowerValidChars =
        {
            'a', 'b', 'c', 'd', 'e', 'f',
        };

        private static readonly char[] LowerInvalidChars =
        {
            'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };

        private static readonly char[] UpperValidChars =
        {
            'A', 'B', 'C', 'D', 'E', 'F',
        };

        private static readonly char[] UpperInvalidChars =
        {
            'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        };

        private static readonly char[] DigitChars =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        };

        #region FromHex

        [Test]
        public void FromHex_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => HexUtil.FromHex(null));
        }

        [Test]
        public void FromHex_GivenInvalidChars_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() =>
                HexUtil.FromHex("aabcfg"));
        }

        [Test]
        public void FromHex_GivenInvalidLength_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() =>
                HexUtil.FromHex("aabcf"));
        }

        [Test]
        public void FromHex_GivenEmptyString_ReturnsEmptyByteArray()
        {
            byte[] bytes = HexUtil.FromHex("");

            Assert.That(bytes, Is.EqualTo(new byte[] { }));
        }

        [Test]
        public void FromHex_GivenAllHexCharsInLowerCase_ReturnsDecodedBytes()
        {
            byte[] bytes = HexUtil.FromHex(
                "000102030405060708090a0b0c0d0e0f" +
                "101112131415161718191a1b1c1d1e1f" +
                "202122232425262728292a2b2c2d2e2f" +
                "303132333435363738393a3b3c3d3e3f" +
                "404142434445464748494a4b4c4d4e4f" +
                "505152535455565758595a5b5c5d5e5f" +
                "606162636465666768696a6b6c6d6e6f" +
                "707172737475767778797a7b7c7d7e7f" +
                "808182838485868788898a8b8c8d8e8f" +
                "909192939495969798999a9b9c9d9e9f" +
                "a0a1a2a3a4a5a6a7a8a9aaabacadaeaf" +
                "b0b1b2b3b4b5b6b7b8b9babbbcbdbebf" +
                "c0c1c2c3c4c5c6c7c8c9cacbcccdcecf" +
                "d0d1d2d3d4d5d6d7d8d9dadbdcdddedf" +
                "e0e1e2e3e4e5e6e7e8e9eaebecedeeef" +
                "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff");


            CollectionAssert.AreEqual(new byte[]
                {
                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c,
                    0x0d, 0x0e, 0x0f,
                    0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c,
                    0x1d, 0x1e, 0x1f,
                    0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c,
                    0x2d, 0x2e, 0x2f,
                    0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c,
                    0x3d, 0x3e, 0x3f,
                    0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c,
                    0x4d, 0x4e, 0x4f,
                    0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c,
                    0x5d, 0x5e, 0x5f,
                    0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c,
                    0x6d, 0x6e, 0x6f,
                    0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c,
                    0x7d, 0x7e, 0x7f,
                    0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c,
                    0x8d, 0x8e, 0x8f,
                    0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c,
                    0x9d, 0x9e, 0x9f,
                    0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac,
                    0xad, 0xae, 0xaf,
                    0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc,
                    0xbd, 0xbe, 0xbf,
                    0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc,
                    0xcd, 0xce, 0xcf,
                    0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc,
                    0xdd, 0xde, 0xdf,
                    0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec,
                    0xed, 0xee, 0xef,
                    0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc,
                    0xfd, 0xfe, 0xff,
                },
                bytes);
        }

        [Test]
        public void FromHex_GivenAllHexCharsInUpperCase_ReturnsDecodedBytes()
        {
            byte[] bytes = HexUtil.FromHex(
                "000102030405060708090A0B0C0D0E0F" +
                "101112131415161718191A1B1C1D1E1F" +
                "202122232425262728292A2B2C2D2E2F" +
                "303132333435363738393A3B3C3D3E3F" +
                "404142434445464748494A4B4C4D4E4F" +
                "505152535455565758595A5B5C5D5E5F" +
                "606162636465666768696A6B6C6D6E6F" +
                "707172737475767778797A7B7C7D7E7F" +
                "808182838485868788898A8B8C8D8E8F" +
                "909192939495969798999A9B9C9D9E9F" +
                "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF" +
                "B0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF" +
                "C0C1C2C3C4C5C6C7C8C9CACBCCCDCECF" +
                "D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF" +
                "E0E1E2E3E4E5E6E7E8E9EAEBECEDEEEF" +
                "F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF");


            CollectionAssert.AreEqual(new byte[]
                {
                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c,
                    0x0d, 0x0e, 0x0f,
                    0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c,
                    0x1d, 0x1e, 0x1f,
                    0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c,
                    0x2d, 0x2e, 0x2f,
                    0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c,
                    0x3d, 0x3e, 0x3f,
                    0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c,
                    0x4d, 0x4e, 0x4f,
                    0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c,
                    0x5d, 0x5e, 0x5f,
                    0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c,
                    0x6d, 0x6e, 0x6f,
                    0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c,
                    0x7d, 0x7e, 0x7f,
                    0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c,
                    0x8d, 0x8e, 0x8f,
                    0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c,
                    0x9d, 0x9e, 0x9f,
                    0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac,
                    0xad, 0xae, 0xaf,
                    0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc,
                    0xbd, 0xbe, 0xbf,
                    0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc,
                    0xcd, 0xce, 0xcf,
                    0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc,
                    0xdd, 0xde, 0xdf,
                    0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec,
                    0xed, 0xee, 0xef,
                    0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc,
                    0xfd, 0xfe, 0xff,
                },
                bytes);
        }

        #endregion

        #region ToHex

        [Test]
        public void ToHex_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                HexUtil.ToHex(null);
            });
        }

        [Test]
        public void ToHex_GivenEmptyArray_ReturnsEmptyString()
        {
            string hex = HexUtil.ToHex(new byte[0]);

            Assert.That(hex, Is.EqualTo(""));
        }

        [Test]
        public void ToHex_Given256Bytes_ReturnsHexStringInUpperCase()
        {
            string actual = HexUtil.ToHex(new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
                0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
                0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
                0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
                0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
                0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
                0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
                0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,
                0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
                0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
                0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,
            }, true);

            string expected =
                "000102030405060708090A0B0C0D0E0F" +
                "101112131415161718191A1B1C1D1E1F" +
                "202122232425262728292A2B2C2D2E2F" +
                "303132333435363738393A3B3C3D3E3F" +
                "404142434445464748494A4B4C4D4E4F" +
                "505152535455565758595A5B5C5D5E5F" +
                "606162636465666768696A6B6C6D6E6F" +
                "707172737475767778797A7B7C7D7E7F" +
                "808182838485868788898A8B8C8D8E8F" +
                "909192939495969798999A9B9C9D9E9F" +
                "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF" +
                "B0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF" +
                "C0C1C2C3C4C5C6C7C8C9CACBCCCDCECF" +
                "D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF" +
                "E0E1E2E3E4E5E6E7E8E9EAEBECEDEEEF" +
                "F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF";

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToHex_Given256Bytes_ReturnsHexStringInLowerCase()
        {
            string actual = HexUtil.ToHex(new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
                0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
                0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
                0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
                0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
                0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
                0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
                0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,
                0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
                0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
                0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,
            });

            string expected =
                "000102030405060708090a0b0c0d0e0f" +
                "101112131415161718191a1b1c1d1e1f" +
                "202122232425262728292a2b2c2d2e2f" +
                "303132333435363738393a3b3c3d3e3f" +
                "404142434445464748494a4b4c4d4e4f" +
                "505152535455565758595a5b5c5d5e5f" +
                "606162636465666768696a6b6c6d6e6f" +
                "707172737475767778797a7b7c7d7e7f" +
                "808182838485868788898a8b8c8d8e8f" +
                "909192939495969798999a9b9c9d9e9f" +
                "a0a1a2a3a4a5a6a7a8a9aaabacadaeaf" +
                "b0b1b2b3b4b5b6b7b8b9babbbcbdbebf" +
                "c0c1c2c3c4c5c6c7c8c9cacbcccdcecf" +
                "d0d1d2d3d4d5d6d7d8d9dadbdcdddedf" +
                "e0e1e2e3e4e5e6e7e8e9eaebecedeeef" +
                "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff";

            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region IsHex

        [Test]
        [TestCaseSource(nameof(UpperValidChars))]
        public void IsHex_GivenMultipleUpperValidChars_ReturnsTrue(char ch)
        {
            Assert.That(HexUtil.IsHex(ch), Is.True);
        }

        [Test]
        [TestCaseSource(nameof(UpperInvalidChars))]
        public void IsHex_GivenMultipleUpperInvalidChars_ReturnsFalse(char ch)
        {
            Assert.That(HexUtil.IsHex(ch), Is.False);
        }

        [Test]
        [TestCaseSource(nameof(LowerValidChars))]
        public void IsHex_GivenMultipleLowerValidChars_ReturnsTrue(char ch)
        {
            Assert.That(HexUtil.IsHex(ch), Is.True);
        }

        [Test]
        [TestCaseSource(nameof(LowerInvalidChars))]
        public void IsHex_GivenMultipleLowerInvalidChars_ReturnsFalse(char ch)
        {
            Assert.That(HexUtil.IsHex(ch), Is.False);
        }

        [Test]
        [TestCaseSource(nameof(DigitChars))]
        public void IsHex_GivenMultipleDigitChars_ReturnsTrue(char ch)
        {
            Assert.That(HexUtil.IsHex(ch), Is.True);
        }

        #endregion
    }
}
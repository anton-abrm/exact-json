// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace ExactJson.Infra
{
    internal static class StringBuilderUtil
    {
        public static void Reverse(StringBuilder sb, int start, int count)
        {
            if (sb is null) {
                throw new ArgumentNullException(nameof(sb));
            }

            for (var i = 0; i < count / 2; i++) {
                Swap(sb, i + start, count - 1 - i + start);
            }
        }

        private static void Swap(StringBuilder sb, int i, int j)
        {
            char tmp = sb[i];
            sb[i] = sb[j];
            sb[j] = tmp;
        }
    }
}
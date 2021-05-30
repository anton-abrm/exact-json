using System;
using System.Text;

namespace ExactJson.Infra
{
    internal static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder _instance;

        private const int MaxCapacity = 360;

        public static StringBuilder Acquire(int capacity = 16)
        {
            if (capacity <= MaxCapacity) {
                var instance = _instance;

                if (instance != null && capacity <= instance.Capacity) {
                    _instance = null;

                    instance.Clear();

                    return instance;
                }
            }

            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb)
        {
            if (sb is null) {
                throw new ArgumentNullException(nameof(sb));
            }

            if (sb.Capacity <= MaxCapacity) {
                _instance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            if (sb is null) {
                throw new ArgumentNullException(nameof(sb));
            }

            var result = sb.ToString();

            Release(sb);

            return result;
        }
    }
}
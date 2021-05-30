using System;
using System.Collections.Generic;

namespace ExactJson.Infra
{
    internal static class CollectionUtil
    {
        public static void CopyTo<T>(ICollection<T> items, T[] array, int arrayIndex)
        {
            if (items is null) {
                throw new ArgumentNullException(nameof(items));
            }

            if (array is null) {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (arrayIndex > array.Length - items.Count) {
                throw new ArgumentException("Not enough space in the output array.");
            }

            foreach (var item in items) {
                array[arrayIndex++] = item;
            }
        }
    }
}
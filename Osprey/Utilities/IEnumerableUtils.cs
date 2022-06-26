using System;
using System.Collections.Generic;
using System.Linq;

namespace Osprey.Utilities
{
    internal static class IEnumerableUtils
    {
        public static T Car<T>(this IEnumerable<T> values)
        {
            try
            {
                return values.First();
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static IEnumerable<T> Cdr<T>(this IEnumerable<T> values)
        {
            try
            {
                return values.Skip(1);
            } catch (Exception ex)
            {
                return Array.Empty<T>();
            }
        }
    }
}

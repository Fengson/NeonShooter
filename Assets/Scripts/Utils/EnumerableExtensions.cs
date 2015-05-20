using System.Collections.Generic;

namespace NeonShooter.Utils
{
    public static class EnumerableExtensions
    {
        public static ExtendedEnumerator<T> GetExtendedEnumerator<T>(this IEnumerable<T> enumerable)
        {
            return new ExtendedEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}

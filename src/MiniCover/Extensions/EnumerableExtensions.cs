using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IGrouping<TKey, T>> GroupByMany<T, TKey>(this IEnumerable<T> source, Func<T, IEnumerable<TKey>> keysSelector)
        {
            return source.SelectMany(x => keysSelector(x), (x, k) => new { x, k })
                .GroupBy(j => j.k, j => j.x);
        }
    }
}

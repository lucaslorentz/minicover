using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace MiniCover.Extensions
{
    public static class ObjectExtensions
    {
        private static ConditionalWeakTable<object, ConcurrentDictionary<object, object>> _extendedDataTable
            = new ConditionalWeakTable<object, ConcurrentDictionary<object, object>>();

        public static T GetOrAddExtension<T>(this object obj, string key, Func<T> factory)
        {
            var extendedData = _extendedDataTable.GetOrCreateValue(obj);
            return (T)extendedData.GetOrAdd(key, k => factory());
        }
    }
}

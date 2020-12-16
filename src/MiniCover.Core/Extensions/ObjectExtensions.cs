using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace MiniCover.Core.Extensions
{
    public static class ObjectExtensions
    {
        private static ConditionalWeakTable<object, ConcurrentDictionary<object, object>> _extendedDataTable
            = new ConditionalWeakTable<object, ConcurrentDictionary<object, object>>();

        public static T GetExtension<T>(this object obj, string key, T defaultValue = default)
        {
            var extendedData = _extendedDataTable.GetOrCreateValue(obj);
            if (!extendedData.TryGetValue(key, out var value))
                return defaultValue;
            return (T)value;
        }

        public static T GetOrAddExtension<T>(this object obj, string key, Func<T> factory)
        {
            var extendedData = _extendedDataTable.GetOrCreateValue(obj);
            return (T)extendedData.GetOrAdd(key, k => factory());
        }

        public static void SetExtension(this object obj, string key, object value)
        {
            var extendedData = _extendedDataTable.GetOrCreateValue(obj);
            extendedData[key] = value;
        }
    }
}

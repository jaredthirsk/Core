﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    // For support of no class constraints, consider http://jonskeet.uk/csharp/miscutil/usage/genericoperators.html

    public static class IDictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if (!dict.ContainsKey(key)) return defaultValue;
            return dict[key];
        }
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
            where TValue : class
        {
            var result = dict.TryGetValue(key);
            if (result == null)
            {
                result = factory(key);
                dict.Add(key, result);
            }
            return result;
        }
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
            where TValue : class
        {
            var result = dict.TryGetValue(key);
            if (result == null)
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = value;
            }
        }
    }
}

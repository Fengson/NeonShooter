using System;
using System.Collections.Generic;

namespace NeonShooter.Utils
{
    public static class CollectionHelper
    {
        public static V TryGet<K,V>(this Dictionary<K,V> dictionary, K key)
        {
            V val;
            bool success = dictionary.TryGetValue(key, out val);
            if (!success) return default(V);
            else return val;
        }

        public static V TryRemove<K,V>(this Dictionary<K,V> dictionary, K key)
            where V : class
        {
            try
            {
                V value = dictionary[key];
                dictionary.Remove(key);
                return value;
            }
            catch (ArgumentNullException)
            {
            }
            catch (KeyNotFoundException)
            {
            }

            return null;
        }

        public static V? TryRemoveValueType<K, V>(this Dictionary<K, V> dictionary, K key)
            where V : struct
        {
            try
            {
                V value = dictionary[key];
                dictionary.Remove(key);
                return value;
            }
            catch (ArgumentNullException)
            {
            }
            catch (KeyNotFoundException)
            {
            }

            return null;
        }
    }
}

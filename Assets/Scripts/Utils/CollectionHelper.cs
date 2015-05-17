using System;
using System.Collections.Generic;

namespace NeonShooter.Utils
{
    public static class CollectionHelper
    {
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

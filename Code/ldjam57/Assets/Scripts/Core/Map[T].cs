﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public class Map<TKey, TValue>
    {
        private readonly IDictionary<TKey, Dictionary<TKey, TValue>> map = new Dictionary<TKey, Dictionary<TKey, TValue>>();

        public TValue this[TKey key1, TKey key2]
        {
            get
            {
                if (map.TryGetValue(key1, out var innerMap))
                {
                    if (innerMap.TryGetValue(key2, out var value))
                    {
                        return value;
                    }
                }

                return default;
            }
            set 
            {
                if (!map.TryGetValue(key1, out var innerMap))
                {
                    innerMap = new Dictionary<TKey, TValue>();

                    map[key1] = innerMap;
                }

                innerMap[key2] = value;
            }
        }

        public Boolean TryGetValue(TKey key1, TKey key2, out TValue value)
        {
            value = default;

            if (map.TryGetValue(key1, out var innerMap))
            {
                if (innerMap.TryGetValue(key2, out value))
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean TryGetValue<TRequestedValue>(TKey key1, TKey key2, out TRequestedValue value) where TRequestedValue : TValue
        {
            value = default;

            if (TryGetValue(key1, key2, out var storedValue))
            {
                if (storedValue is TRequestedValue castedValue)
                {
                    value = castedValue;
                    return true;
                }
            }

            return false;
        }

        public Boolean Remove(TKey key1, TKey key2)
        { 
            if (map.TryGetValue(key1, out var innerMap))
            {
                return innerMap.Remove(key2);
            }

            return false;
        }

        public IEnumerable<TValue> GetAll()
        {
            foreach (var innerMap in map.Values)
            {
                foreach (var item in innerMap.Values)
                {
                    yield return item;
                }
            }
        }
    }
}

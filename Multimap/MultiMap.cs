﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Multimap
{
    public class NullNotAllowedException : ApplicationException
    {
        public NullNotAllowedException(string msg) : base(msg) { }
    };

    public delegate bool ValueCheck<V>(V newValue);

    public class MultiMap<K, V> : IMultiMap<K, V>
    {
        //all keys
        public IEnumerable<K> Keys => map.Keys;

        //all values
        public IEnumerable<V> Values
        {
            get
            {
                foreach (KeyValuePair<K, List<V>> pair in map)
                {
                    foreach (V value in pair.Value)
                    {
                        yield return value;
                    }
                };
            }
        }

        //all values for ONE key
        public IEnumerable<V> this[K key]
        {
            get
            {
                List<V> tempValues;
                if (map.TryGetValue(key, out tempValues))
                    return tempValues;
                else
                    return new List<V>();
            }
        }

        private Dictionary<K, List<V>> map = new Dictionary<K, List<V>>();
        private ValueCheck<V> isValueNull;

        public event ChangeKey<K> RemoveKey;
        public event ChangeKey<K> AddKey;
        public event ChangeValue<V> AddValue;
        public event ChangeValue<V> RemoveValue;
        public event CheckKeyValuePair<K, V> DeleteKeyValuePair;

        public MultiMap(ValueCheck<V> _valueCheck)
        {
            isValueNull = _valueCheck;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (KeyValuePair<K, List<V>> pair in map)
            {
                foreach (V value in pair.Value)
                {
                    yield return new KeyValuePair<K, V>(pair.Key, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(K _key)
        {
            if (map.ContainsKey(_key))
                return true;
            else
                return false;
        }

        public bool ContainsValue(K _key, V _value)
        {
            List<V> tempValues;

            //out is specified in TryGetValue, so programm knows it has to FILL this list with the values it finds
            if (map.TryGetValue(_key, out tempValues))
                return tempValues.Contains(_value);
            else
                return false;
        }

        public void Add(K _key, V _newValue)
        {
            if (isValueNull(_newValue))
                throw new NullNotAllowedException("Null not allowed as value");

            List<V> tempValues;

            if (map.TryGetValue(_key, out tempValues))
            {
                AddValue?.Invoke(_newValue);
                tempValues.Add(_newValue);
            }
            else
            {
                AddKey?.Invoke(_key);

                tempValues = new List<V>();
                tempValues.Add(_newValue);
                map.Add(_key, tempValues);
            }
        }

        public void Remove(K _key, V _ValueToRemove)
        {
            List<V> tempValues;

            if (map.TryGetValue(_key, out tempValues))
            {
                if (tempValues.Remove(_ValueToRemove))
                {
                    RemoveValue?.Invoke(_ValueToRemove);

                    if (tempValues.Count == 0)
                    {
                        RemoveKey?.Invoke(_key);
                        map.Remove(_key);
                    }
                }
                else
                    Console.WriteLine("The value " + _ValueToRemove + " could not be found in association with " + _key);
            }
            else
            {
                Console.WriteLine("The key " + _key + " could not be found");
            }
        }

        public void Add<K2, V2>(IMultiMap<K2, V2> newMultiMap)
            where K2 : K
            where V2 : V
        {
            foreach (K2 newKey in newMultiMap.Keys)
            {
                foreach (V2 newValue in newMultiMap[newKey])
                    Add(newKey, newValue);
            }
        }

        public void RemoveAll(CheckKeyValuePair<K, V> checkKeyValuePair)
        {
            List<K> keysToDelete = new List<K>();

            foreach (KeyValuePair<K, List<V>> pair in map)
            {
                pair.Value.RemoveAll(value => checkKeyValuePair(pair.Key, value));

                if (pair.Value.Count == 0)
                {
                    keysToDelete.Add(pair.Key);
                }
            }

            foreach (K key in keysToDelete)
            {
                map.Remove(key);
            }
        }
    }
}
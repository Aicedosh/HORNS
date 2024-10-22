﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class IdSet<T> : IEnumerable<T> where T : IIdentifiable
    {
        private protected Dictionary<int, T> elements = new Dictionary<int, T>();

        private class TComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(T var)
            {
                return var.Id.GetHashCode();
            }
        }

        internal virtual bool Add(T v)
        {
            if (elements.ContainsKey(v.Id)) return false;
            elements.Add(v.Id, v);
            return true;
        }

        internal void Replace(T v)
        {
            elements[v.Id] = v;
        }

        internal void RemoveWhere(Func<T, bool> condition)
        {
            var toRemove = new List<int>();
            foreach (var element in elements)
            {
                if (condition(element.Value))
                {
                    toRemove.Add(element.Key);
                }
            }
            foreach (var key in toRemove)
            {
                elements.Remove(key);
            }
        }

        internal bool TryGet(ref T v)
        {
            bool res;
            if (res = elements.TryGetValue(v.Id, out T var))
            {
                v = var;
            }
            return res;
        }

        internal T this[int id] => elements[id];
        internal bool Contains(int id) => elements.ContainsKey(id);

        internal IdSet<T> Clone()
        {
            IdSet<T> set = new IdSet<T>();
            foreach(T e in elements.Values)
            {
                set.Add((T)e.GetCopy());
            }
            return set;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return elements.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.Values.GetEnumerator();
        }
    }
}

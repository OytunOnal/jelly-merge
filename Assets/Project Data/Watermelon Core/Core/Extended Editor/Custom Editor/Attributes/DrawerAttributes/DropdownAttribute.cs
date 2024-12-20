﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DropdownAttribute : DrawerAttribute
    {
        private string valuesFieldName;

        public DropdownAttribute(string valuesFieldName)
        {
            this.valuesFieldName = valuesFieldName;
        }

        public string ValuesFieldName
        {
            get
            {
                return this.valuesFieldName;
            }
        }
    }

    public interface IDropdownList : IEnumerable<KeyValuePair<string, object>>
    {
    }

    public class DropdownList<T> : IDropdownList
    {
        private List<KeyValuePair<string, object>> values;

        public DropdownList()
        {
            this.values = new List<KeyValuePair<string, object>>();
        }

        public void Add(string displayName, T value)
        {
            this.values.Add(new KeyValuePair<string, object>(displayName, value));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static explicit operator DropdownList<object>(DropdownList<T> target)
        {
            DropdownList<object> result = new DropdownList<object>();
            foreach (var kvp in target)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}

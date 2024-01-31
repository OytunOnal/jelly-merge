using System;
using UnityEngine;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableFieldAttribute : PropertyAttribute
    {
        public DisableFieldAttribute()
        {

        }
    }
}

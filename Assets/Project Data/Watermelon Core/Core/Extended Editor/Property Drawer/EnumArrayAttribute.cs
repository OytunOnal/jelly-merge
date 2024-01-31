using System;
using UnityEngine;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumArrayAttribute : PropertyAttribute
    {
        public Type selectedEnum;

        public EnumArrayAttribute(Type selectedEnum)
        {
            this.selectedEnum = selectedEnum;
        }
    }
}
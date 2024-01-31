using System;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SelectorAnchorAttribute : Attribute
    {
        public SelectorAnchorAttribute()
        {
        }
    }
}

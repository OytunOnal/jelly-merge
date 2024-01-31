using System;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ErrorAttribute : HelpBoxAttribute
    {
        public ErrorAttribute(string title) : base(title)
        {

        }
    }
}
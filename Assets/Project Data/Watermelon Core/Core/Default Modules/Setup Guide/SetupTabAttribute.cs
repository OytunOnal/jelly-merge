using System;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SetupTabAttribute : Attribute
    {
        public string tabName;

        public string texture;
        public int priority = int.MaxValue;
        public bool showScrollView = true;

        public SetupTabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}

// -----------------
// Setup Guide v 1.0.1
// -----------------
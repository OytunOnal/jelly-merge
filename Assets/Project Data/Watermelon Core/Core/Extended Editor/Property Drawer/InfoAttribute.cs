﻿using System;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InfoAttribute : HelpBoxAttribute
    {
        public InfoAttribute(string title) : base(title)
        {

        }
    }
}

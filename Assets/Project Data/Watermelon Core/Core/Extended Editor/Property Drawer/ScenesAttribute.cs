﻿using System;
using UnityEngine;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScenesAttribute : PropertyAttribute
    {
        public ScenesAttribute()
        {

        }
    }
}
using System;

namespace JellyMerge
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowNonSerializedFieldAttribute : DrawerAttribute
    {
    }
}

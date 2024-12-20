using System;
using System.Collections.Generic;

namespace JellyMerge
{
    public static class NativePropertyDrawerDatabase
    {
        private static Dictionary<Type, NativePropertyDrawer> drawersByAttributeType;

        static NativePropertyDrawerDatabase()
        {
            drawersByAttributeType = new Dictionary<Type, NativePropertyDrawer>();
            drawersByAttributeType[typeof(ShowNativePropertyAttribute)] = new ShowNativePropertyNativePropertyDrawer();

        }

        public static NativePropertyDrawer GetDrawerForAttribute(Type attributeType)
        {
            NativePropertyDrawer drawer;
            if (drawersByAttributeType.TryGetValue(attributeType, out drawer))
            {
                return drawer;
            }
            else
            {
                return null;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace JellyMerge
{
    public static class PropertyDrawerDatabase
    {
        private static Dictionary<Type, PropertyDrawer> drawersByAttributeType;

        static PropertyDrawerDatabase()
        {
            drawersByAttributeType = new Dictionary<Type, PropertyDrawer>();
            drawersByAttributeType[typeof(DropdownAttribute)] = new DropdownPropertyDrawer();
            drawersByAttributeType[typeof(MinMaxSliderAttribute)] = new MinMaxSliderPropertyDrawer();
            drawersByAttributeType[typeof(ProgressBarAttribute)] = new ProgressBarPropertyDrawer();
            drawersByAttributeType[typeof(ReadOnlyFieldAttribute)] = new ReadOnlyFieldPropertyDrawer();
            drawersByAttributeType[typeof(ReorderableListAttribute)] = new ReorderableListPropertyDrawer();
            drawersByAttributeType[typeof(ResizableTextAreaAttribute)] = new ResizableTextAreaPropertyDrawer();
            drawersByAttributeType[typeof(ShowAssetPreviewAttribute)] = new ShowAssetPreviewPropertyDrawer();
            drawersByAttributeType[typeof(SliderAttribute)] = new SliderPropertyDrawer();
        }

        public static PropertyDrawer GetDrawerForAttribute(Type attributeType)
        {
            PropertyDrawer drawer;
            if (drawersByAttributeType.TryGetValue(attributeType, out drawer))
            {
                return drawer;
            }
            else
            {
                return null;
            }
        }

        public static void ClearCache()
        {
            foreach (var kvp in drawersByAttributeType)
            {
                kvp.Value.ClearCache();
            }
        }
    }
}
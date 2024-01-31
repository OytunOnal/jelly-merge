using UnityEditor;

namespace JellyMerge
{
    public abstract class PropertyMeta
    {
        public abstract void ApplyPropertyMeta(SerializedProperty property, MetaAttribute metaAttribute);
    }
}

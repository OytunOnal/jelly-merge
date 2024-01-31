using UnityEditor;

namespace JellyMerge
{
    public abstract class PropertyValidator
    {
        public abstract void ValidateProperty(SerializedProperty property);
    }
}

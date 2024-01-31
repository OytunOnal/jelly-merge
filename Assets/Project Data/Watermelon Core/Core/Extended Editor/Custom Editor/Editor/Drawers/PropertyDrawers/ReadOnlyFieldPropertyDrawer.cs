using UnityEditor;
using UnityEngine;

namespace JellyMerge
{
    [PropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public class ReadOnlyFieldPropertyDrawer : PropertyDrawer
    {
        public override void DrawProperty(SerializedProperty property)
        {
            GUI.enabled = false;
            EditorDrawUtility.DrawPropertyField(property);
            GUI.enabled = true;
        }
    }
}

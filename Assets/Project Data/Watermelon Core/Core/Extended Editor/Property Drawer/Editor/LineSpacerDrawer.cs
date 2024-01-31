using UnityEngine;
using UnityEditor;

namespace JellyMerge
{
    [CustomPropertyDrawer(typeof(LineSpacerAttribute))]
    public class LineSpacerDrawer : DecoratorDrawer
    {
        private LineSpacerAttribute lineSpacer
        {
            get { return (LineSpacerAttribute)attribute; }
        }

        public override void OnGUI(Rect position)
        {
            LineSpacerAttribute lineSpacer = this.lineSpacer;

            Color oldGuiColor = GUI.color;
            EditorGUI.LabelField(new Rect(position.x, position.y + lineSpacer.height - 12, position.width, lineSpacer.height), lineSpacer.title, EditorStyles.boldLabel);
            EditorGUI.LabelField(new Rect(position.x, position.y + lineSpacer.height, position.width, lineSpacer.height), "", GUI.skin.horizontalSlider);
            GUI.color = oldGuiColor;
        }

        public override float GetHeight()
        {
            return base.GetHeight() + lineSpacer.height;
        }
    }
}
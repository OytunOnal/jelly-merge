#pragma warning disable 649

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public class CellTypesHandler
    {
        private const string SELECTED = " (Selected)";
        private const int BUTTON_HEIGHT = 24;

        public int selectedCellTypeValue;
        public List<CellType> cellTypes;
        public List<ExtraProp> extraProps;
        private GUIStyle labelStyle;

        public GUIStyle LabelStyle { get => labelStyle; set => labelStyle = value; }

        public CellTypesHandler()
        {
            cellTypes = new List<CellType>();
            extraProps = new List<ExtraProp>();
        }

        public void AddCellType(CellType cellType)
        {
            cellTypes.Add(cellType);
        }

        public void AddExtraProp(ExtraProp extraProp)
        {
            extraProps.Add(extraProp);
        }

        public void DrawCellButtons()
        {
            foreach(CellType cellType in cellTypes)
            {
                DrawCellButton(cellType);
            }
        }

        public void SetDefaultLabelStyle()
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        public void SetDefaultLabelStyle(Color color)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = color;
        }

        private void DrawCellButton(CellType cellType)
        {
            Rect rect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Space(BUTTON_HEIGHT);
            EditorGUILayout.EndVertical();

            if(LevelEditorBase.Instance == null)
            {
                Debug.LogError("LevelEditorBase.Instance == null.");
            }

            if(labelStyle == null)
            {
                Debug.LogError("labelStyle of GridHandler is null.");
            }

            LevelEditorBase.DrawColorRect(rect, cellType.color);

            if(selectedCellTypeValue == cellType.value)
            {
                GUI.Label(rect, cellType.label +  SELECTED, labelStyle);
            }
            else
            {
                GUI.Label(rect, cellType.label, labelStyle);
            }

            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                selectedCellTypeValue = cellType.value;
            }
        }

        private Color GetDisabledColor(Color color)
        {
            return color.SetAlpha(0.4f);
        }

        public CellType GetCellType(int value)
        {
            for (int i = 0; i < cellTypes.Count; i++)
            {
                if(cellTypes[i].value == value)
                {
                    return cellTypes[i];
                }
            }

            return null;
        }

        public ExtraProp GetExtraProp(int value)
        {
            for (int i = 0; i < extraProps.Count; i++)
            {
                if (extraProps[i].value == value)
                {
                    return extraProps[i];
                }
            }

            return null;
        }

        public class CellType
        {
            public int value;
            public string label;
            public Color color;
            public bool extraPropsEnabled;

            public CellType(int value, string label, Color color,bool extraPropsEnabled = false)
            {
                this.value = value;
                this.label = label;
                this.color = color;
                this.extraPropsEnabled = extraPropsEnabled;
            }
        }

        public class ExtraProp
        {
            public int value;
            public string label;
            public bool display;

            public ExtraProp(int value, string label, bool display = true)
            {
                this.value = value;
                this.label = label;
                this.display = display;
            }
        }
    }
}
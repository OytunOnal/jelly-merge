#pragma warning disable 649

using UnityEngine;
using UnityEditor;
using System;
using Watermelon.JellyMerge;

namespace Watermelon
{
    public class LevelEditorWindow : LevelEditorBase
    {

        //used variables
        private const string LEVELS_PROPERTY_NAME = "levels";
        private SerializedProperty levelsSerializedProperty;
        private LevelRepresentation selectedLevelRepresentation;
        private LevelsHandler levelsHandler;
        private CellTypesHandler gridHandler;

        //sidebar
        private const float LIST_AREA_WIDTH = 320f;

        //instructions
    private const string LEVEL_INSTRUCTION = "Level should have at least 2 color cells of the same type (Color1, Color2 or Color3).";
        private const string RIGHT_CLICK_INSTRUCTION = "You can use right click to switch between celltypes.";
        private const int INFO_HEIGH = 122; //found out using Debug.Log(infoRect) on worst case scenario
        private const string LEVEL_PASSED_VALIDATION = "Level passed validation.";
        private Rect infoRect;

        //level drawing
        private Rect drawRect;
        private float xSize;
        private float ySize;
        private float elementSize;
        private Event currentEvent;
        private Vector2 elementUnderMouseIndex;
        private Vector2Int elementPosition;
        private int invertedY;
        private float buttonRectX;
        private float buttonRectY;
        private Rect buttonRect;

        //Menu
        private int menuIndex1;
        private int menuIndex2;

        protected override WindowConfiguration SetUpWindowConfiguration(WindowConfiguration.Builder builder)
        {
            return builder.SetWindowMinSize(new Vector2(700, 500)).Build();
        }

        protected override Type GetLevelsDatabaseType()
        {
            return typeof(LevelsDatabase);
        }

        public override Type GetLevelType()
        {
            return typeof(Level);
        }

        protected override void ReadLevelDatabaseFields()
        {
            levelsSerializedProperty = levelsDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);
        }

        protected override void InitialiseVariables()
        {
            levelsHandler = new LevelsHandler(levelsDatabaseSerializedObject, levelsSerializedProperty);
            gridHandler = new CellTypesHandler();
            gridHandler.AddCellType(new CellTypesHandler.CellType(0, ColorId.None.ToString(), new Color(0.4f, 0.4f, 0.4f)));
            gridHandler.AddCellType(new CellTypesHandler.CellType(1, ColorId.Static.ToString(), new Color(0.25f, 0.25f, 0.25f)));
            gridHandler.AddCellType(new CellTypesHandler.CellType(2, ColorId.Color1.ToString(), new Color(1f, 0.5f, 0.3f,0.8f)));
            gridHandler.AddCellType(new CellTypesHandler.CellType(3, ColorId.Color2.ToString(), new Color(0.8f, 1f, 0.75f, 0.8f)));
            gridHandler.AddCellType(new CellTypesHandler.CellType(4, ColorId.Color3.ToString(), new Color(0.5f, 0.8f, 1f, 0.8f)));
           
        }

        protected override void Styles()
        {
            if (gridHandler != null)
            {
                gridHandler.SetDefaultLabelStyle();
            }
        }

        public override void OpenLevel(UnityEngine.Object levelObject, int index)
        {
            selectedLevelRepresentation = new LevelRepresentation(levelObject);
        }

        public override string GetLevelLabel(UnityEngine.Object levelObject, int index)
        {
            return new LevelRepresentation(levelObject).GetLevelLabel(index, stringBuilder);
        }

        public override void ClearLevel(UnityEngine.Object levelObject)
        {
            new LevelRepresentation(levelObject).Clear();
        }
        public override void LogErrorsForGlobalValidation(UnityEngine.Object levelObject, int index)
        {
            LevelRepresentation level = new LevelRepresentation(levelObject);
            level.ValidateLevel();

            if (!level.IsLevelCorrect)
            {
                Debug.Log("Logging validation errors for level #" + (index + 1) + " :");

                foreach (string error in level.errorLabels)
                {
                    Debug.LogWarning(error);
                }
            }
            else
            {
                Debug.Log($"Level #{(index + 1)} passed validation.");
            }
        }

        protected override void DrawContent()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DisplayListArea();
            EditorGUILayout.Space();
            DisplayMainArea();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void DisplayListArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(LIST_AREA_WIDTH));
            levelsHandler.DisplayReordableList();
            levelsHandler.DrawRenameLevelsButton();
            levelsHandler.DrawGlobalValidationButton();
            gridHandler.DrawCellButtons();
            EditorGUILayout.EndVertical();
        }

        private void DisplayMainArea()
        {

            if (levelsHandler.SelectedLevelIndex == -1)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            if (IsPropertyChanged(levelsHandler.SelectedLevelProperty, new GUIContent("File")))
            {
                levelsHandler.ReopenLevel();
            }

            if (IsPropertyChanged(selectedLevelRepresentation.sizeProperty))
            {
                selectedLevelRepresentation.HandleSizePropertyChange();
            }

            if(GUILayout.Button("Test Level"))
            {
                TestLevel();
            }

            DrawLevel();

            levelsHandler.UpdateCurrentLevelLabel(selectedLevelRepresentation.GetLevelLabel(levelsHandler.SelectedLevelIndex, stringBuilder));
            selectedLevelRepresentation.ApplyChanges();

            DrawTipsAndWarnings();

            EditorGUILayout.EndVertical();
        }

        private void TestLevel()
        {
            GlobalSave globalSave = SaveController.GetGlobalSave();
            globalSave.GetSaveObject<SimpleIntSave>("current_level_index").Value = levelsHandler.SelectedLevelIndex;
            SaveController.SaveCustom(globalSave);

            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        private void DrawLevel()
        {
            drawRect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            xSize = Mathf.Floor(drawRect.width / selectedLevelRepresentation.sizeProperty.vector2IntValue.x);
            ySize = Mathf.Floor(drawRect.height / selectedLevelRepresentation.sizeProperty.vector2IntValue.y);
            elementSize = Mathf.Min(xSize, ySize);
            currentEvent = Event.current;
            CellTypesHandler.CellType cellType;

            //Handle drag and click
            if ((currentEvent.type == EventType.MouseDrag) || (currentEvent.type == EventType.MouseDown))
            {
                elementUnderMouseIndex = (currentEvent.mousePosition - drawRect.position) / (elementSize);

                elementPosition = new Vector2Int(Mathf.FloorToInt(elementUnderMouseIndex.x), selectedLevelRepresentation.sizeProperty.vector2IntValue.y - 1 - Mathf.FloorToInt(elementUnderMouseIndex.y));

                if ((elementPosition.x >= 0) && (elementPosition.x < selectedLevelRepresentation.sizeProperty.vector2IntValue.x) && (elementPosition.y >= 0) && (elementPosition.y < selectedLevelRepresentation.sizeProperty.vector2IntValue.y))
                {
                    if (currentEvent.button == 0)
                    {
                        selectedLevelRepresentation.SetItemsValue(elementPosition.x, elementPosition.y, gridHandler.selectedCellTypeValue);
                        currentEvent.Use();
                    }
                    else if ((currentEvent.button == 1) && (currentEvent.type == EventType.MouseDown))
                    {
                        cellType = gridHandler.GetCellType(selectedLevelRepresentation.GetItemsValue(elementPosition.x, elementPosition.y));

                        GenericMenu menu = new GenericMenu();

                        menuIndex1 = elementPosition.x;
                        menuIndex2 = elementPosition.y;

                        foreach (CellTypesHandler.CellType el in gridHandler.cellTypes)
                        {
                            menu.AddItem(new GUIContent(el.label), el.value == cellType.value, ()=> gridHandler.selectedCellTypeValue = el.value);
                        }

                        menu.ShowAsContext();
                    }
                }
            }

            //draw
            for (int y = selectedLevelRepresentation.sizeProperty.vector2IntValue.y - 1; y >= 0; y--)
            {
                invertedY = selectedLevelRepresentation.sizeProperty.vector2IntValue.y - 1 - y;

                for (int x = 0; x < selectedLevelRepresentation.sizeProperty.vector2IntValue.x; x++)
                {
                    cellType = gridHandler.GetCellType(selectedLevelRepresentation.GetItemsValue(x, y));


                    buttonRectX = drawRect.position.x + x * elementSize;
                    buttonRectY = drawRect.position.y + invertedY * elementSize;
                    buttonRect = new Rect(buttonRectX, buttonRectY, elementSize, elementSize);

                    DrawColorRect(buttonRect, cellType.color);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void DrawTipsAndWarnings()
        {
            infoRect = EditorGUILayout.BeginVertical(GUILayout.MinHeight(INFO_HEIGH));
            EditorGUILayout.HelpBox(LEVEL_INSTRUCTION, MessageType.Info);
            EditorGUILayout.HelpBox(RIGHT_CLICK_INSTRUCTION, MessageType.Info);

            if (selectedLevelRepresentation.IsLevelCorrect)
            {
                EditorGUILayout.HelpBox(LEVEL_PASSED_VALIDATION, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(selectedLevelRepresentation.errorLabels[0], MessageType.Error);
            }

            EditorGUILayout.EndVertical();

            //Debug.Log(infoRect.height);
        }


        //this 2 methods prevent editor from closing in play mode
        public override bool WindowClosedInPlaymode()
        {
            return false;
        }

        public override void OnBeforeAssemblyReload()
        {
        }

        protected class LevelRepresentation : LevelRepresentationBase
        {
            private const string SIZE_PROPERTY_NAME = "size";
            private const string ITEMS_PROPERTY_NAME = "items";
            private const string INTS_PROPERTY_NAME = "ints";
            private const string NO_COLOR_CELLS_WARNING = "Level don`t have color cells.";
            public SerializedProperty sizeProperty;
            public SerializedProperty itemsProperty;

            protected override bool LEVEL_CHECK_ENABLED => true;

            public LevelRepresentation(UnityEngine.Object levelObject) : base(levelObject)
            {
            }

            protected override void ReadFields()
            {
                sizeProperty = serializedLevelObject.FindProperty(SIZE_PROPERTY_NAME);
                itemsProperty = serializedLevelObject.FindProperty(ITEMS_PROPERTY_NAME);
            }

            public override void Clear()
            {
                sizeProperty.vector2IntValue = Vector2Int.zero;
                itemsProperty.arraySize = 0;
                ApplyChanges();
            }

            public int GetItemsValue(int index1, int index2)
            {
                return itemsProperty.GetArrayElementAtIndex(index1).FindPropertyRelative(INTS_PROPERTY_NAME).GetArrayElementAtIndex(index2).intValue;
            }

            public void SetItemsValue(int index1, int index2, int newValue)
            {
                itemsProperty.GetArrayElementAtIndex(index1).FindPropertyRelative(INTS_PROPERTY_NAME).GetArrayElementAtIndex(index2).intValue = newValue;
            }

            public void HandleSizePropertyChange()
            {
                if (sizeProperty.vector2IntValue.x < 2)
                {
                    sizeProperty.vector2IntValue = new Vector2Int(2, sizeProperty.vector2IntValue.y);
                }

                if (sizeProperty.vector2IntValue.y < 2)
                {
                    sizeProperty.vector2IntValue = new Vector2Int(sizeProperty.vector2IntValue.x, 2);
                }

                itemsProperty.arraySize = sizeProperty.vector2IntValue.x;

                for (int x = 0; x < sizeProperty.vector2IntValue.x; x++)
                {
                    itemsProperty.GetArrayElementAtIndex(x).FindPropertyRelative(INTS_PROPERTY_NAME).arraySize = sizeProperty.vector2IntValue.y;
                }
            }

            public override void ValidateLevel()
            {
                errorLabels.Clear();
                int tempCellType;

                bool colorCellExist = false; //Check if at least 1 color cell exist

                int[] colorsAmount = new int[Enum.GetNames(typeof(ColorId)).Length];

                for (int i = 0; i < colorsAmount.Length; i++)
                {
                    colorsAmount[i] = 0;
                }

                for (int i = 0; i < sizeProperty.vector2IntValue.x; i++)
                {
                    for (int j = 0; j < sizeProperty.vector2IntValue.y; j++)
                    {
                        tempCellType = GetItemsValue(i, j);
                        //counting amounts of all colors
                        if ((tempCellType != (int)ColorId.Static) && (tempCellType != (int)ColorId.None))
                        {
                            colorCellExist = true;
                            colorsAmount[tempCellType]++;
                        }
                    }
                }

                if (!colorCellExist)
                {
                    errorLabels.Add(NO_COLOR_CELLS_WARNING);
                }

                for (int i = 0; i < colorsAmount.Length; i++)
                {
                    if (colorsAmount[i] == 1)
                    {
                        errorLabels.Add($"Only one cell with {(ColorId)i} .");
                    }
                }
            }
        }
    }
}

// -----------------
// 2d grid level editor
// -----------------

// Changelog
// v 1.2
// • Reordered some methods
// v 1.1
// • Added global validation
// • Added validation example
// • Fixed mouse click bug
// v 1 basic version works
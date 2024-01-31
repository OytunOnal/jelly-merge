using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace JellyMerge
{
    [CustomEditor(typeof(ColorsController))]
    public class ColorsControllerEditor : Editor
    {
        //private ColorsPreset colorsPreset;
        private bool presetCreation = false;
        private string newPresetName;

        GUIStyle boldSyle = new GUIStyle();
        ColorsController controller;

        private const string PRESETS_PATH = @"Assets/Project Data/Content/ColorPresets/";

        private void OnEnable()
        {
            boldSyle.fontStyle = FontStyle.Bold;
            presetCreation = false;
            newPresetName = string.Empty;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            controller = target as ColorsController;

            controller.color1Material = DrawMaterialAndColorEditor("Color1", controller.color1Material);
            controller.color2Material = DrawMaterialAndColorEditor("Color2", controller.color2Material);
            controller.color3Material = DrawMaterialAndColorEditor("Color3", controller.color3Material);

            GUILayout.Space(5);

            controller.bordersMaterial = DrawMaterialAndColorEditor("Borders", controller.bordersMaterial);
            controller.backplaneMaterial = DrawMaterialAndColorEditor("Bottom", controller.backplaneMaterial);

            GUILayout.Space(5);

            if (controller.currentPresetEditor != null)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                controller.currentPresetEditor.uiLighter = DrawUIColorEditor("UI Lighter", controller.currentPresetEditor.uiLighter);
                controller.currentPresetEditor.uiDarker = DrawUIColorEditor("UI Darker", controller.currentPresetEditor.uiDarker);
                GUILayout.EndVertical();

                if (EditorApplication.isPlaying)
                {
                    if (GUILayout.Button("Show UI", GUILayout.Height(34), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)))
                    {
                        UIComplete.InitUIColors(controller.currentPresetEditor);
                        ColorsController.ShowLevelCompleteEditor();
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.Label("Developement", boldSyle);

            if (!presetCreation)
            {
                EditorGUILayout.BeginHorizontal();
                controller.currentPresetEditor = (ColorsPreset)EditorGUILayout.ObjectField("Current Preset: ", controller.currentPresetEditor, typeof(ColorsPreset), true);

                if (GUILayout.Button("New"))
                {
                    presetCreation = true;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                if (EditorApplication.isPlaying)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("◀", GUILayout.Height(20)))
                    {
                        ColorsController.EditorSetRandomPresetInverse();
                    }

                    if (GUILayout.Button("▶", GUILayout.Height(20)))
                    {
                        ColorsController.SetRandomPreset();
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Load current", GUILayout.Height(30), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.463f)))
                {
                    LoadCurrent(controller);
                }

                if (GUILayout.Button("Save to current", GUILayout.Height(30), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.463f)))
                {
                    SaveToCurrent(controller);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                newPresetName = EditorGUILayout.TextField("Preset Name: ", newPresetName);

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Cancel", GUILayout.Height(30)))
                {
                    presetCreation = false;
                    newPresetName = string.Empty;
                }

                if (GUILayout.Button("Create", GUILayout.Height(30)))
                {
                    controller.currentPresetEditor = CreateInstance<ColorsPreset>();
                    AssetDatabase.CreateAsset(controller.currentPresetEditor, PRESETS_PATH + (newPresetName == string.Empty ? "NewPreset" : newPresetName) + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    presetCreation = false;
                }

                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Material DrawMaterialAndColorEditor(string label, Material material)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            Material newMaterial = (Material)EditorGUILayout.ObjectField(label, material, typeof(Material), false);
            newMaterial.color = EditorGUILayout.ColorField(newMaterial.color, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));

            if (GUI.changed || newMaterial.color != material.color)
            {
                Undo.RecordObject(target, "Changed color preset");

                if (!EditorApplication.isPlaying)
                {
                    EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                }
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            return newMaterial;
        }

        private Color DrawUIColorEditor(string label, Color color)
        {
            EditorGUI.BeginChangeCheck();

            Color newColor = EditorGUILayout.ColorField(label, color);

            if (GUI.changed || newColor != color)
            {
                Undo.RecordObject(target, "Changed color preset");

                EditorUtility.SetDirty(controller.currentPresetEditor);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return newColor;
        }


        [Button("Load current")]
        public void LoadCurrent(ColorsController controller)
        {
            if (controller.currentPresetEditor != null)
            {
                controller.SetPreset(controller.currentPresetEditor);
            }
            else
            {
                Debug.Log("Please, assign current preset");
            }
        }

        [Button("Save to current")]
        public void SaveToCurrent(ColorsController controller)
        {
            if (controller.currentPresetEditor != null)
            {
                controller.currentPresetEditor.color1 = controller.color1Material.color;
                controller.currentPresetEditor.color2 = controller.color2Material.color;
                controller.currentPresetEditor.color3 = controller.color3Material.color;
                controller.currentPresetEditor.borders = controller.bordersMaterial.color;
                controller.currentPresetEditor.backplane = controller.backplaneMaterial.color;

                EditorUtility.SetDirty(controller.currentPresetEditor);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Please, assign current preset");
            }
        }
    }
}
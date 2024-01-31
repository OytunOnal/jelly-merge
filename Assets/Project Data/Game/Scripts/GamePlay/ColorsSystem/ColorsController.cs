using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

namespace Watermelon.JellyMerge
{
    /// <summary>
    /// Controlls colors of all materials used at game. Use static SetRandomPreset or inst SetPreset() to change color theme.
    /// </summary>
    public class ColorsController : MonoBehaviour
    {
        private static ColorsController instance;

        [Header("References")]
        public List<ColorsPreset> presetsList;

        [HideInInspector]
        public Material color1Material;
        [HideInInspector]
        public Material color2Material;
        [HideInInspector]
        public Material color3Material;
        [HideInInspector]
        public Material bordersMaterial;
        [HideInInspector]
        public Material backplaneMaterial;

        public static ColorsPreset CurrentPreset
        {
            get { return instance.presetsList[instance.currentPresetIndex >= 0 ? instance.currentPresetIndex : 0]; }
        }

        private int currentPresetIndex = 0;

#if UNITY_EDITOR
        [HideInInspector]
        public ColorsPreset currentPresetEditor;
#endif

        public delegate void ColorsEvent();
        public static ColorsEvent OnColorsPresetChanged;

        private void Awake()
        {
            instance = this;

            presetsList.Shuffle();
        }

        public static void SetRandomPreset()
        {
            instance.SetPreset(instance.presetsList[instance.currentPresetIndex]);
        }

        public void SetPreset(ColorsPreset colorsPreset)
        {
            instance.currentPresetIndex++;

            if (instance.currentPresetIndex >= instance.presetsList.Count)
            {
                instance.currentPresetIndex = 0;
            }

            color1Material.color = colorsPreset.color1;
            color2Material.color = colorsPreset.color2;
            color3Material.color = colorsPreset.color3;
            bordersMaterial.color = colorsPreset.borders;

            UIComplete.InitUIColors(colorsPreset);

            backplaneMaterial.color = colorsPreset.backplane;

            OnColorsPresetChanged?.Invoke();

#if UNITY_EDITOR
            currentPresetEditor = colorsPreset;
#endif
        }

        public static Material GetColorMaterial(ColorId colorId)
        {
            if (colorId == ColorId.Color1)
            {
                return instance.color1Material;
            }
            else if (colorId == ColorId.Color2)
            {
                return instance.color2Material;
            }
            else
            {
                return instance.color3Material;
            }
        }

#if UNITY_EDITOR
        public static void EditorSetRandomPresetInverse()
        {
            instance.currentPresetIndex--;

            if (instance.currentPresetIndex < 0)
            {
                instance.currentPresetIndex = instance.presetsList.Count - 1;
            }

            instance.SetPreset(instance.presetsList[instance.currentPresetIndex]);
        }
#endif

        public static void ShowLevelCompleteEditor()
        {
            instance.StartCoroutine(instance.EditorUIPreviewCoroutine());
        }

        private IEnumerator EditorUIPreviewCoroutine()
        {
            UIController.ShowPage<UIComplete>();
            UIController.HidePage<UIGame>();

            yield return new WaitForSeconds(1.5f);

            UIController.ShowPage<UIGame>();
            UIController.HidePage<UIComplete>();
        }

    }
}
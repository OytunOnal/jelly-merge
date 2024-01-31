using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JellyMerge
{
    [CreateAssetMenu(fileName = "ColorsPreset", menuName = "Games/JelljMerge/ColorsSystem/ColorsPreset")]
    public class ColorsPreset : ScriptableObject
    {
        public Color borders = Color.white;
        public Color backplane = Color.white;

        [Space(5)]
        public Color color1 = Color.white;
        public Color color2 = Color.white;
        public Color color3 = Color.white;

        [Space(5)]
        public Color uiLighter = Color.white;
        public Color uiDarker = Color.white;
    }
}
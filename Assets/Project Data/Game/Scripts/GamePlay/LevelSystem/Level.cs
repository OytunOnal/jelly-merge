using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon.JellyMerge
{
    [CreateAssetMenu(fileName = "Level", menuName = "Games/JelljMerge/LevelSystem/Level")]
    [System.Serializable]
    public class Level : ScriptableObject
    {
        public Vector2Int size;

        public IntArray[] items;

        [System.Serializable]
        public struct IntArray
        {
            public int[] ints;

            public IntArray(int[] list)
            {
                ints = list;
            }
        }

    }
}
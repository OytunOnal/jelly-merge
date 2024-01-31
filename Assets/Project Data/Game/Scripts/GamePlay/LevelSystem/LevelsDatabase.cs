﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace JellyMerge
{
    [CreateAssetMenu(fileName = "NewLevelsDatabase", menuName = "Games/JelljMerge/LevelSystem/LevelDatabase")]
    public class LevelsDatabase : ScriptableObject
    {
        private static LevelsDatabase instance;

        [SerializeField] private Level[] levels;

        public void Init()
        {
            instance = this;
        }

        public static int LevelsCount
        {
            get { return instance.levels.Length; }
        }

        public static Level GetLevel(int index)
        {
            return instance.levels[index % instance.levels.Length];
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon.TrainMatch
{
    [CreateAssetMenu(fileName = "Lives Data", menuName = "Content/Data/Lives")]
    public class LivesData : ScriptableObject
    {
        public int maxLivesCount;
        [Tooltip("In seconds")]public int oneLifeRestorationDuration;

        [Space]
        public string fullText = "FULL!";
        public string timespanFormat = "{mm\\:ss}";
    }
}
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Easing Settings", menuName = "Games/JelljMerge/Settings/Tween Easing Settings")]
    public class EasingSettings : ScriptableObject
    {
        [SerializeField] CustomEasingFunction[] customEasingFunctions;
        public CustomEasingFunction[] CustomEasingFunctions => customEasingFunctions;
    }
}


// -----------------
// Tween v 1.3.1
// -----------------
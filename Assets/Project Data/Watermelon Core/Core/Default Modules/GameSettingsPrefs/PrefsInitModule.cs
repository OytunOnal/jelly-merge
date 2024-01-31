using UnityEngine;

namespace JellyMerge
{
    [RegisterModule("Core/Prefs Settings")]
    public class PrefsInitModule : InitModule
    {
        [SerializeField] PrefsSettings prefsSettings;

        public override void CreateComponent(Initialiser Initialiser)
        {
            prefsSettings.Init();
        }

        public PrefsInitModule()
        {
            moduleName = "Prefs Settings";
        }
    }
}

// -----------------
// Prefs Settings v1.0
// -----------------
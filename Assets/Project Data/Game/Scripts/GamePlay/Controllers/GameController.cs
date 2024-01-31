using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Watermelon;

namespace Watermelon.JellyMerge
{
    public class GameController : MonoBehaviour
    {
        private static GameController instance;

        [SerializeField] LevelsDatabase levelsDatabase;
        [SerializeField] UIController uiController;

        [Space(5)]
        public int levelNumberDev = 1;

        private Level currentLevel;

        private SimpleIntSave currentLevelIndexSave;

        public static int CurrentLevelIndex
        {
            get { return instance.currentLevelIndexSave.Value; }
            private set { instance.currentLevelIndexSave.Value = value; }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            SaveController.Initialise(true);
            currentLevelIndexSave = SaveController.GetSaveObject<SimpleIntSave>("current_level_index");

            levelsDatabase.Init();

            uiController.Initialise();
            uiController.InitialisePages();

            StartGame();
        }

        private void StartGame()
        {
            currentLevel = LevelsDatabase.GetLevel(CurrentLevelIndex);

            LevelController.Load(currentLevel);

            UIController.ShowPage<UIMainMenu>();

            ColorsController.SetRandomPreset();

            if (CurrentLevelIndex < 2)
            {
                StartCoroutine(TutorialCoroutine());
            }

            GameLoading.MarkAsReadyToHide();
        }

        public static void OnLevelComplete()
        {
            // Play swipe sound
            AudioController.PlaySound(AudioController.Sounds.gameWinClip);

            UIController.HidePage<UIGame>();
            UIController.ShowPage<UIComplete>();

            CurrentLevelIndex++;
            SaveController.MarkAsSaveIsRequired();

            if (CurrentLevelIndex < 2)
            {
                UIController.GamePage.HideTutorialPanel();
            }

            Tween.DelayedCall(1.5f, delegate
            {
                
                    UIController.HidePage<UIComplete>();
                    instance.StartGame();
            });
        }

        private IEnumerator TutorialCoroutine()
        {
            if (CurrentLevelIndex == 0)
            {
                UIController.GamePage.ShowTutorialPanel(false);
                yield break;
            }
            else if (CurrentLevelIndex == 1)
            {
                yield return new WaitForSeconds(4f);
                UIController.GamePage.ShowTutorialPanel(true);
            }

            WaitForSeconds delay = new WaitForSeconds(0.5f);

            while (CurrentLevelIndex < 2)
            {
                yield return delay;
            }

            UIController.GamePage.HideTutorialPanel();
        }

        public static void OnTapPerformed()
        {
            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIGame>();
        }

        public static void Restart()
        {
            LevelController.Restart();
        }

        [Button("Load level")]
        public void LoadLevelDev()
        {
            CurrentLevelIndex = Mathf.Clamp(levelNumberDev - 1, 0, int.MaxValue);

            currentLevel = LevelsDatabase.GetLevel(CurrentLevelIndex);
            LevelController.Load(currentLevel);

            UIController.ShowPage<UIMainMenu>();
            UIController.HidePage<UIGame>();
            UIController.HidePage<UIComplete>();

            if (CurrentLevelIndex < 2)
            {
                StartCoroutine(TutorialCoroutine());
            }
        }

        public static void LoadLevelDev(int index)
        {
            if (index >= 2)
            {
                UIController.GamePage.HideTutorialPanel();
            }

            instance.levelNumberDev = index + 1;
            instance.LoadLevelDev();
        }
    }
}
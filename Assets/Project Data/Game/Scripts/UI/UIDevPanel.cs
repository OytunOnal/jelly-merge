using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace JellyMerge
{
    public class UIDevPanel : MonoBehaviour
    {
        [SerializeField] Button resetProgressButton;
        [SerializeField] Button prevLevelButton;
        [SerializeField] Button nextLevelButton;
        [SerializeField] Button changeColorButton;
        [SerializeField] Button hideButton;

        private void Awake()
        {
            resetProgressButton.onClick.AddListener(ResetProgressButton);
            prevLevelButton.onClick.AddListener(PrevLevelButton);
            nextLevelButton.onClick.AddListener(NextLevelButton);
            changeColorButton.onClick.AddListener(ChangeColorButton);
            hideButton.onClick.AddListener(HideDevButtons);
        }

        public void ResetProgressButton()
        {
            SaveController.GetSaveObject<SimpleIntSave>("current_level_index").Value = 0;
            GameController.LoadLevelDev(0);
        }

        public void PrevLevelButton()
        {
            int currentLevelIndex = SaveController.GetSaveObject<SimpleIntSave>("current_level_index").Value;
            currentLevelIndex--;

            if (currentLevelIndex < 0)
            {
                currentLevelIndex = LevelsDatabase.LevelsCount - 1;
            }

            SaveController.GetSaveObject<SimpleIntSave>("current_level_index").Value = currentLevelIndex;
            GameController.LoadLevelDev(currentLevelIndex);
        }

        public void NextLevelButton()
        {
            int currentLevelIndex = SaveController.GetSaveObject<SimpleIntSave>("current_level_index").Value;
            currentLevelIndex++;

            if (currentLevelIndex > LevelsDatabase.LevelsCount - 1)
            {
                currentLevelIndex = 0;
            }

            SaveController.GetSaveObject<SimpleIntSave>("current_level_index").Value = currentLevelIndex;
            GameController.LoadLevelDev(currentLevelIndex);
        }

        public void ChangeColorButton()
        {
            ColorsController.SetRandomPreset();
        }


        public void HideDevButtons()
        {
            gameObject.SetActive(false);
        }

    }
}